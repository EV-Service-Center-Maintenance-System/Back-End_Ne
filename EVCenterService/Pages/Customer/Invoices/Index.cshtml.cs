using EVCenterService.Data; 
using EVCenterService.Models; 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore; 
using System.Collections.Generic; 
using System.Linq; 
using System.Security.Claims; 
using System.Threading.Tasks; 

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
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                return RedirectToPage("/Account/Login");
            }

            InvoiceList = await _context.Invoices
                .Include(i => i.Order)
                    .ThenInclude(o => o.Vehicle)
                .Include(i => i.Subscription) 
                    .ThenInclude(s => s.Plan) 
                .Where(i => (i.Order != null && i.Order.UserId == userId) ||
                            (i.Subscription != null && i.Subscription.UserId == userId))
                .OrderByDescending(i => i.IssueDate)
                .ToListAsync();

            return Page();
        }
    }
}