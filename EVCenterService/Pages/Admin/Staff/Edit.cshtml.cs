using EVCenterService.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AccountEntity = EVCenterService.Models.Account;

namespace Test.Pages.Admin.Staff
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly EVServiceCenterContext _context;
        private readonly IPasswordHasher<AccountEntity> _passwordHasher;

        public EditModel(EVServiceCenterContext context, IPasswordHasher<AccountEntity> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        [BindProperty]
        public AccountEntity StaffAccount { get; set; } = default!;

        [BindProperty]
        public string? NewPassword { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null) return NotFound();

            var account = await _context.Accounts.FirstOrDefaultAsync(m => m.UserId == id);

            if (account == null) return NotFound();

            StaffAccount = account;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var accountToUpdate = await _context.Accounts.FindAsync(StaffAccount.UserId);
            if (accountToUpdate == null) return NotFound();

            accountToUpdate.FullName = StaffAccount.FullName;
            accountToUpdate.Email = StaffAccount.Email;
            accountToUpdate.Phone = StaffAccount.Phone;
            accountToUpdate.Role = StaffAccount.Role;

            if (!string.IsNullOrWhiteSpace(NewPassword))
            {
                accountToUpdate.Password = _passwordHasher.HashPassword(accountToUpdate, NewPassword);
            }

            await _context.SaveChangesAsync();

            TempData["StatusMessage"] = $"Thông tin c?a {accountToUpdate.FullName} ?ã ???c c?p nh?t.";
            return RedirectToPage("./Index");
        }
    }
}
