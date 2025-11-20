using EVCenterService.Data;
using EVCenterService.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace EVCenterService.Pages.Customer.Invoices
{
    [Authorize(Roles = "Customer")]
    public class PaymentCallbackModel : PageModel
    {
        private readonly IVnPayService _vnPayService;
        private readonly EVServiceCenterContext _context;
        private readonly IMemoryCache _memoryCache;
        private readonly IEmailSender _emailSender;

        public PaymentCallbackModel(IVnPayService vnPayService, EVServiceCenterContext context, IMemoryCache memoryCache, IEmailSender emailSender)
        {
            _vnPayService = vnPayService;
            _context = context;
            _memoryCache = memoryCache;
            _emailSender = emailSender;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            if (response == null)
            {
                TempData["ErrorMessage"] = "Phản hồi thanh toán không hợp lệ.";
                return RedirectToPage("/Customer/Index");
            }

            var paymentAttemptId = response.OrderId;

            if (!_memoryCache.TryGetValue(paymentAttemptId, out int invoiceId))
            {
                TempData["ErrorMessage"] = "Không thể xác minh giao dịch hoặc phiên thanh toán đã hết hạn.";
 
                return RedirectToPage("./Index");
            }

            _memoryCache.Remove(paymentAttemptId);

            if (response.Success && response.VnPayResponseCode == "00")
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var invoice = await _context.Invoices
                        .Include(i => i.Order)
                        .Include(i => i.Subscription)
                            .ThenInclude(s => s.Plan)
                        .Include(i => i.Subscription.User)
                        .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);

                    if (invoice != null && invoice.Status == "Unpaid")
                    {
                        invoice.Status = "Paid";

                        if (invoice.OrderId != null && invoice.Order != null)
                        {
                            invoice.Order.Status = "ReadyForRepair";
                            TempData["StatusMessage"] = $"Thanh toán thành công cho Hóa Đơn Dịch Vụ #{invoiceId}.";
                        }
                        else if (invoice.SubscriptionId != null && invoice.Subscription != null)
                        {
                            invoice.Subscription.Status = "active";
                            TempData["StatusMessage"] = $"Đăng ký gói {invoice.Subscription.Plan.Name} thành công!";

                            var user = invoice.Subscription.User;
                            var subject = "Cảm ơn bạn đã đăng ký gói dịch vụ EV Center";
                            var message = $@"
                            <p>Chào {user.FullName},</p>
                            <p>Cảm ơn bạn đã đăng ký thành công gói <strong>{invoice.Subscription.Plan.Name}</strong>.</p>
                            <p>Gói dịch vụ của bạn có hiệu lực từ {invoice.Subscription.StartDate:dd/MM/yyyy} đến {invoice.Subscription.EndDate:dd/MM/yyyy}.</V>
                            <p>Trân trọng,<br> EV Service Center</p>";

                            await _emailSender.SendEmailAsync(user.Email, subject, message);
                        }

                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                    }
                    else if (invoice != null && invoice.Status == "Paid")
                    {
                        TempData["StatusMessage"] = "Hóa đơn này đã được thanh toán.";
                    }
                    else if (invoice == null)
                    {
                        TempData["ErrorMessage"] = $"Không tìm thấy hóa đơn #{invoiceId} trong hệ thống.";
                        return RedirectToPage("./Index"); 
                    }

                    return RedirectToPage("./Details", new { id = invoiceId });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    TempData["ErrorMessage"] = $"Lỗi khi cập nhật CSDL: {ex.Message}";
                    return RedirectToPage("./Details", new { id = invoiceId });
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Thanh toán không thành công. Mã lỗi VNPay: " + response.VnPayResponseCode;
                return RedirectToPage("./Details", new { id = invoiceId });
            }
        }
    }
}