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

        // OnGet vì VNPay tr? v? b?ng QueryString
        public async Task<IActionResult> OnGetAsync()
        {
            // 1. Th?c thi thanh toán
            var response = _vnPayService.PaymentExecute(Request.Query);

            if (response == null)
            {
                TempData["ErrorMessage"] = "Ph?n h?i thanh toán không h?p l?.";
                return RedirectToPage("/Customer/Index"); // V? trang ch? customer
            }

            // 1. L?y paymentAttemptId t? ph?n h?i VNPay (nó n?m trong response.OrderId)
            var paymentAttemptId = response.OrderId;

            // 2. Tra c?u InvoiceId th?t t? Cache
            if (!_memoryCache.TryGetValue(paymentAttemptId, out int invoiceId))
            {
                // Không tìm th?y mapping trong cache (có th? h?t h?n ho?c l?i)
                TempData["ErrorMessage"] = "Không th? xác minh giao d?ch ho?c phiên thanh toán ?ã h?t h?n.";
                // Chuy?n h??ng v? trang Index c?a hóa ??n thay vì Details
                return RedirectToPage("./Index");
            }
            // Optional: Xóa kh?i cache sau khi l?y ???c ?? gi?i phóng b? nh?
            _memoryCache.Remove(paymentAttemptId);

            // 2. Ki?m tra k?t qu? thanh toán
            if (response.Success && response.VnPayResponseCode == "00")
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // Dùng invoiceId l?y t? cache ?? tìm hóa ??n
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
                            // === LOGIC C? (CHO D?CH V? S?A CH?A) ===
                            invoice.Order.Status = "ReadyForRepair";
                            TempData["StatusMessage"] = $"Thanh toán thành công cho Hóa ??n D?ch V? #{invoiceId}.";
                        }
                        else if (invoice.SubscriptionId != null && invoice.Subscription != null)
                        {
                            // === LOGIC M?I (CHO GÓI D?CH V?) ===
                            invoice.Subscription.Status = "active";
                            TempData["StatusMessage"] = $"??ng ký gói {invoice.Subscription.Plan.Name} thành công!";

                            // G?I EMAIL C?M ?N
                            var user = invoice.Subscription.User;
                            var subject = "C?m ?n b?n ?ã ??ng ký gói d?ch v? EV Center";
                            var message = $@"
                            <p>Chào {user.FullName},</p>
                            <p>C?m ?n b?n ?ã ??ng ký thành công gói <strong>{invoice.Subscription.Plan.Name}</strong>.</p>
                            <p>Gói d?ch v? c?a b?n có hi?u l?c t? {invoice.Subscription.StartDate:dd/MM/yyyy} ??n {invoice.Subscription.EndDate:dd/MM/yyyy}.</V>
                            <p>Trân tr?ng,<br>??i ng? EV Service Center</p>";

                            await _emailSender.SendEmailAsync(user.Email, subject, message);
                        }

                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                    }
                    else if (invoice != null && invoice.Status == "Paid")
                    {
                        TempData["StatusMessage"] = "Hóa ??n này ?ã ???c thanh toán.";
                    }
                    else if (invoice == null)
                    {
                        TempData["ErrorMessage"] = $"Không tìm th?y hóa ??n #{invoiceId} trong h? th?ng.";
                        return RedirectToPage("./Index"); // Không có hóa ??n thì v? trang Index
                    }

                    // Chuy?n h??ng v? trang Details c?a ?úng hóa ??n
                    return RedirectToPage("./Details", new { id = invoiceId });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    TempData["ErrorMessage"] = $"L?i khi c?p nh?t CSDL: {ex.Message}";
                    // N?u l?i CSDL, c?ng v? trang Details ?? ng??i dùng th?y hóa ??n
                    return RedirectToPage("./Details", new { id = invoiceId });
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Thanh toán không thành công. Mã l?i VNPay: " + response.VnPayResponseCode;
                // N?u thanh toán l?i, c?ng v? trang Details
                return RedirectToPage("./Details", new { id = invoiceId });
            }
        }
    }
}