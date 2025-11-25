using EVCenterService.Data;
using EVCenterService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVCenterService.Pages.Admin.Invoices
{
    [Authorize(Roles = "Admin, Staff")]
    public class IndexModel : PageModel
    {
        private readonly EVServiceCenterContext _context;

        public IndexModel(EVServiceCenterContext context)
        {
            _context = context;
        }

        public IList<Invoice> InvoiceList { get; set; } = new List<Invoice>();

        public async Task OnGetAsync()
        {
            InvoiceList = await _context.Invoices
                .Include(i => i.Subscription)
                    .ThenInclude(s => s.User)
                .OrderByDescending(i => i.IssueDate)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}