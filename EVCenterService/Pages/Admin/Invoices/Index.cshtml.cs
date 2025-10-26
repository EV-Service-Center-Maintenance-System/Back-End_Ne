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
            // T?m th?i t?i t?t c? h�a ??n t? DB
            // L?U �: D?a tr�n file EVServiceCenterContext.cs, b?ng Invoice c?a b?n ?ang
            // li�n k?t v?i Subscription, kh�ng ph?i OrderService.
            // ?�y l� v?n ?? thi?t k? b?n c?n s?a l?i.
            // Logic t?m th?i s? t?i c�c h�a ??n c?a Subscription.
            InvoiceList = await _context.Invoices
                .Include(i => i.Subscription)
                    .ThenInclude(s => s.User)
                .OrderByDescending(i => i.IssueDate)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}