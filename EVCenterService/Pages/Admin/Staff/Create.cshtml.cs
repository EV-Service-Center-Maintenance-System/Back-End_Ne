using EVCenterService.Data;
using EVCenterService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AccountEntity = EVCenterService.Models.Account;

namespace EVCenterService.Pages.Admin.Staff
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly EVServiceCenterContext _context;
        private readonly IPasswordHasher<AccountEntity> _passwordHasher;

        public CreateModel(EVServiceCenterContext context, IPasswordHasher<AccountEntity> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        [BindProperty]
        public AccountEntity StaffAccount { get; set; } = new();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            StaffAccount.Password = _passwordHasher.HashPassword(StaffAccount, StaffAccount.Password);
            StaffAccount.Status = "Active";

            _context.Accounts.Add(StaffAccount);
            await _context.SaveChangesAsync();

            TempData["StatusMessage"] = $"T?o nhân viên m?i '{StaffAccount.FullName}' thành công.";
            return RedirectToPage("./Index");
        }
    }
}
