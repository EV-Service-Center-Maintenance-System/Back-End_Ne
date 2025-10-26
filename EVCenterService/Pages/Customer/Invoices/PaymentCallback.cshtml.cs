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

        public PaymentCallbackModel(IVnPayService vnPayService, EVServiceCenterContext context, IMemoryCache memoryCache)
        {
            _vnPayService = vnPayService;
            _context = context;
            _memoryCache = memoryCache;
        }

        // OnGet v� VNPay tr? v? b?ng QueryString
        public async Task<IActionResult> OnGetAsync()
        {
            // 1. Th?c thi thanh to�n
            var response = _vnPayService.PaymentExecute(Request.Query);

            if (response == null)
            {
                TempData["ErrorMessage"] = "Ph?n h?i thanh to�n kh�ng h?p l?.";
                return RedirectToPage("/Customer/Index"); // V? trang ch? customer
            }

            // 1. L?y paymentAttemptId t? ph?n h?i VNPay (n� n?m trong response.OrderId)
            var paymentAttemptId = response.OrderId;

            // 2. Tra c?u InvoiceId th?t t? Cache
            if (!_memoryCache.TryGetValue(paymentAttemptId, out int invoiceId))
            {
                // Kh�ng t�m th?y mapping trong cache (c� th? h?t h?n ho?c l?i)
                TempData["ErrorMessage"] = "Kh�ng th? x�c minh giao d?ch ho?c phi�n thanh to�n ?� h?t h?n.";
                // Chuy?n h??ng v? trang Index c?a h�a ??n thay v� Details
                return RedirectToPage("./Index");
            }
            // Optional: X�a kh?i cache sau khi l?y ???c ?? gi?i ph�ng b? nh?
            _memoryCache.Remove(paymentAttemptId);

            // 2. Ki?m tra k?t qu? thanh to�n
            if (response.Success && response.VnPayResponseCode == "00")
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // D�ng invoiceId l?y t? cache ?? t�m h�a ??n
                    var invoice = await _context.Invoices
                        .Include(i => i.Order)
                        .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);

                    if (invoice != null && invoice.Status == "Unpaid")
                    {
                        invoice.Status = "Paid";
                        if (invoice.Order != null)
                        {
                            invoice.Order.Status = "ReadyForRepair";
                        }
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        TempData["StatusMessage"] = $"Thanh to�n th�nh c�ng cho H�a ??n #{invoiceId}.";
                    }
                    else if (invoice != null && invoice.Status == "Paid")
                    {
                        TempData["StatusMessage"] = "H�a ??n n�y ?� ???c thanh to�n.";
                    }
                    else if (invoice == null)
                    {
                        TempData["ErrorMessage"] = $"Kh�ng t�m th?y h�a ??n #{invoiceId} trong h? th?ng.";
                        return RedirectToPage("./Index"); // Kh�ng c� h�a ??n th� v? trang Index
                    }

                    // Chuy?n h??ng v? trang Details c?a ?�ng h�a ??n
                    return RedirectToPage("./Details", new { id = invoiceId });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    TempData["ErrorMessage"] = $"L?i khi c?p nh?t CSDL: {ex.Message}";
                    // N?u l?i CSDL, c?ng v? trang Details ?? ng??i d�ng th?y h�a ??n
                    return RedirectToPage("./Details", new { id = invoiceId });
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Thanh to�n kh�ng th�nh c�ng. M� l?i VNPay: " + response.VnPayResponseCode;
                // N?u thanh to�n l?i, c?ng v? trang Details
                return RedirectToPage("./Details", new { id = invoiceId });
            }
        }
    }
}