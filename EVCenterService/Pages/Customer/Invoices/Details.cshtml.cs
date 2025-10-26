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

            // T?i h�a ??n V� c�c d? li?u li�n quan ?? hi?n th?
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

            // ??m b?o kh�ch h�ng n�y s? h?u h�a ??n
            if (Invoice.Order?.UserId.ToString() != userId)
            {
                return Forbid();
            }

            return Page();
        }

        // Handler cho n�t "Thanh to�n VNPay"
        public async Task<IActionResult> OnPostPayWithVnPayAsync(int id)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Order)
                .FirstOrDefaultAsync(i => i.InvoiceId == id);

            if (invoice == null || !invoice.Amount.HasValue)
            {
                return NotFound();
            }

            if (invoice.Status == "Paid") // Ki?m tra th�m n?u ?� thanh to�n th� kh�ng cho t?o link n?a
            {
                TempData["ErrorMessage"] = "H�a ??n n�y ?� ???c thanh to�n.";
                return RedirectToPage("./Details", new { id = invoice.InvoiceId });
            }

            // 1. T?o m?t m� duy nh?t cho l?n th? thanh to�n n�y (Payment Attempt ID)
            var paymentAttemptId = Guid.NewGuid().ToString("N"); // T?o GUID kh�ng c� d?u g?ch n?i

            // 2. L?u mapping: paymentAttemptId -> InvoiceId v�o Cache
            // ??t th?i gian h?t h?n (v� d?: 15 ph�t) ?? tr�nh cache b? ??y
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(15));
            _memoryCache.Set(paymentAttemptId, invoice.InvoiceId, cacheEntryOptions);

            // T?o model th�ng tin thanh to�n
            var paymentModel = new PaymentInformationModel
            {
                Amount = (double)invoice.Amount.Value,
                // S?A: G?i paymentAttemptId l�m m� giao d?ch
                InvoiceId = paymentAttemptId,
                OrderDescription = $"Thanh toan cho don hang #{invoice.OrderId}",
                Name = "EV Center Service",
                OrderType = "other" // Ho?c lo?i h�nh c?a b?n
            };

            // T?o URL v� chuy?n h??ng ng??i d�ng
            var paymentUrl = _vnPayService.CreatePaymentUrl(paymentModel, HttpContext);
            return Redirect(paymentUrl);
        }
    }
}