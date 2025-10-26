using EVCenterService.Data; // Th�m
using EVCenterService.Models; // Th�m
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore; // Th�m
using System.Collections.Generic; // Th�m
using System.Linq; // Th�m
using System.Security.Claims; // Th�m
using System.Threading.Tasks; // Th�m

namespace EVCenterService.Pages.Customer.Invoices
{
    [Authorize(Roles = "Customer")]
    public class IndexModel : PageModel
    {
        private readonly EVServiceCenterContext _context;

        public IndexModel(EVServiceCenterContext context)
        {
            _context = context;
        }

        public IList<Invoice> InvoiceList { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            // L?y ID c?a ng??i d�ng ?ang ??ng nh?p
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                // X? l� tr??ng h?p kh�ng l?y ???c User ID (v� d?: chuy?n h??ng v? trang ??ng nh?p)
                return RedirectToPage("/Account/Login");
            }

            // Truy v?n h�a ??n c?a ng??i d�ng n�y
            // Bao g?m c? OrderService v� User ?? hi?n th? th�ng tin
            InvoiceList = await _context.Invoices
                .Include(i => i.Order)       // Bao g?m th�ng tin ??n h�ng
                    .ThenInclude(o => o.User) // T? ??n h�ng, l?y th�ng tin ng??i d�ng
                .Include(i => i.Order)       // Bao g?m l?i ??n h�ng
                    .ThenInclude(o => o.Vehicle) // T? ??n h�ng, l?y th�ng tin xe
                .Where(i => i.Order.UserId == userId) // Ch? l?y h�a ??n c?a ng??i d�ng n�y
                .OrderByDescending(i => i.IssueDate)   // S?p x?p m?i nh?t l�n ??u
                .ToListAsync();

            return Page();
        }
    }
}