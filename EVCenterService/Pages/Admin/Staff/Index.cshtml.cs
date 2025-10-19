using EVCenterService.Data;
using EVCenterService.Models;
using EVCenterService.Models.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AccountEntity = EVCenterService.Models.Account;

namespace Test.Pages.Admin.Staff
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly EVServiceCenterContext _context;

        public IndexModel(EVServiceCenterContext context)
        {
            _context = context;
        }

        // S?A 3: Dùng bí danh AccountEntity
        public IList<AccountEntity> StaffList { get; set; } = new List<AccountEntity>();

        public async Task OnGetAsync()
        {
            string staffRole = RoleEnum.Staff.ToString();
            string technicianRole = RoleEnum.Technican.ToString();

            StaffList = await _context.Accounts
                .Where(a => a.Role == staffRole || a.Role == technicianRole)
                .OrderBy(a => a.FullName)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostToggleStatusAsync(Guid id)
        {
            var account = await _context.Accounts.FindAsync(id);

            if (account != null)
            {
                account.Status = (account.Status == "Active") ? "Locked" : "Active";
                await _context.SaveChangesAsync();
                TempData["StatusMessage"] = $"Tr?ng thái c?a tài kho?n {account.FullName} ?ã ???c c?p nh?t.";
            }

            return RedirectToPage();
        }
    }
}
