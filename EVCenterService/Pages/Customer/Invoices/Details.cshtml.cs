using EVCenterService.Data;
using EVCenterService.Models; 
using EVCenterService.Service.Interfaces;
using EVCenterService.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore; 
using System.Security.Claims;
using Microsoft.Extensions.Caching.Memory;

namespace EVCenterService.Pages.Customer.Invoices
{
    [Authorize(Roles = "Customer")]
    public class DetailsModel : PageModel
    {
        private readonly EVServiceCenterContext _context; 
        private readonly IVnPayService _vnPayService;
        private readonly IMemoryCache _memoryCache;

        public DetailsModel(EVServiceCenterContext context, IVnPayService vnPayService, IMemoryCache memoryCache) 
        {
            _context = context;
            _vnPayService = vnPayService;
            _memoryCache = memoryCache;
        }

        [BindProperty]
        public Invoice Invoice { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // T?i hóa ??n VÀ các d? li?u liên quan ?? hi?n th?
            Invoice = await _context.Invoices
                .Include(i => i.Order)
                    .ThenInclude(o => o.User)
                .Include(i => i.Order)
                    .ThenInclude(o => o.Vehicle)
                .Include(i => i.Order)
                    .ThenInclude(o => o.OrderDetails)
                    .ThenInclude(od => od.Service)
                .Include(i => i.Order)
                    .ThenInclude(o => o.PartsUseds)
                    .ThenInclude(pu => pu.Part)
                .FirstOrDefaultAsync(i => i.InvoiceId == id);

            if (Invoice == null)
            {
                return NotFound();
            }

            // ??m b?o khách hàng này s? h?u hóa ??n
            if (Invoice.Order?.UserId.ToString() != userId)
            {
                return Forbid();
            }

            return Page();
        }

        // Handler cho nút "Thanh toán VNPay"
        public async Task<IActionResult> OnPostPayWithVnPayAsync(int id)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Order)
                .FirstOrDefaultAsync(i => i.InvoiceId == id);

            if (invoice == null || !invoice.Amount.HasValue)
            {
                return NotFound();
            }

            if (invoice.Status == "Paid") // Ki?m tra thêm n?u ?ã thanh toán thì không cho t?o link n?a
            {
                TempData["ErrorMessage"] = "Hóa ??n này ?ã ???c thanh toán.";
                return RedirectToPage("./Details", new { id = invoice.InvoiceId });
            }

            // 1. T?o m?t mã duy nh?t cho l?n th? thanh toán này (Payment Attempt ID)
            var paymentAttemptId = Guid.NewGuid().ToString("N"); // T?o GUID không có d?u g?ch n?i

            // 2. L?u mapping: paymentAttemptId -> InvoiceId vào Cache
            // ??t th?i gian h?t h?n (ví d?: 15 phút) ?? tránh cache b? ??y
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(15));
            _memoryCache.Set(paymentAttemptId, invoice.InvoiceId, cacheEntryOptions);

            // T?o model thông tin thanh toán
            var paymentModel = new PaymentInformationModel
            {
                Amount = (double)invoice.Amount.Value,
                // S?A: G?i paymentAttemptId làm mã giao d?ch
                InvoiceId = paymentAttemptId,
                OrderDescription = $"Thanh toan cho don hang #{invoice.OrderId}",
                Name = "EV Center Service",
                OrderType = "other" // Ho?c lo?i hình c?a b?n
            };

            // T?o URL và chuy?n h??ng ng??i dùng
            var paymentUrl = _vnPayService.CreatePaymentUrl(paymentModel, HttpContext);
            return Redirect(paymentUrl);
        }
    }
}