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
                .Include(i => i.Subscription)
                    .ThenInclude(s => s.Plan)
                .FirstOrDefaultAsync(i => i.InvoiceId == id);

            if (Invoice == null)
            {
                return NotFound();
            }

            var subscriptionUserId = await _context.Subscriptions
                                .Where(s => s.SubscriptionId == Invoice.SubscriptionId)
                                .Select(s => s.UserId)
                                .FirstOrDefaultAsync();

            if (Invoice.Order?.UserId.ToString() != userId &&
                subscriptionUserId.ToString() != userId)
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

            if (invoice.Status == "Paid") 
            {
                TempData["ErrorMessage"] = "Hóa ??n này ?ã ???c thanh toán.";
                return RedirectToPage("./Details", new { id = invoice.InvoiceId });
            }

            var paymentAttemptId = Guid.NewGuid().ToString("N"); 

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(15));
            _memoryCache.Set(paymentAttemptId, invoice.InvoiceId, cacheEntryOptions);

            var paymentModel = new PaymentInformationModel
            {
                Amount = (double)invoice.Amount.Value,
                InvoiceId = paymentAttemptId,
                OrderDescription = $"Thanh toan cho don hang #{invoice.OrderId}",
                Name = "EV Center Service",
                OrderType = "other" 
            };

            var paymentUrl = _vnPayService.CreatePaymentUrl(paymentModel, HttpContext);
            return Redirect(paymentUrl);
        }
    }
}