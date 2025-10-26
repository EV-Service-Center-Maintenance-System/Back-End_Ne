using EVCenterService.Data; // Thêm
using EVCenterService.Models; // Thêm
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore; // Thêm
using System.Collections.Generic; // Thêm
using System.Linq; // Thêm
using System.Security.Claims; // Thêm
using System.Threading.Tasks; // Thêm

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
            // L?y ID c?a ng??i dùng ?ang ??ng nh?p
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                // X? lý tr??ng h?p không l?y ???c User ID (ví d?: chuy?n h??ng v? trang ??ng nh?p)
                return RedirectToPage("/Account/Login");
            }

            // Truy v?n hóa ??n c?a ng??i dùng này
            // Bao g?m c? OrderService và User ?? hi?n th? thông tin
            InvoiceList = await _context.Invoices
                .Include(i => i.Order)       // Bao g?m thông tin ??n hàng
                    .ThenInclude(o => o.User) // T? ??n hàng, l?y thông tin ng??i dùng
                .Include(i => i.Order)       // Bao g?m l?i ??n hàng
                    .ThenInclude(o => o.Vehicle) // T? ??n hàng, l?y thông tin xe
                .Where(i => i.Order.UserId == userId) // Ch? l?y hóa ??n c?a ng??i dùng này
                .OrderByDescending(i => i.IssueDate)   // S?p x?p m?i nh?t lên ??u
                .ToListAsync();

            return Page();
        }
    }
}