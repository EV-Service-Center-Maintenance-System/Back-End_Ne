using EVCenterService.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using AccountEntity = EVCenterService.Models.Account;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace EVCenterService.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly EVServiceCenterContext _context;
        private readonly IPasswordHasher<AccountEntity> _passwordHasher;

        public LoginModel(EVServiceCenterContext context, IPasswordHasher<AccountEntity> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required][EmailAddress] public string Email { get; set; }
            [Required][DataType(DataType.Password)] public string Password { get; set; }
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Email == Input.Email);

            if (account == null || _passwordHasher.VerifyHashedPassword(account, account.Password, Input.Password) == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(string.Empty, "Email ho?c m?t kh?u không h?p l?.");
                return Page();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, account.Email),
                new Claim(ClaimTypes.GivenName, account.FullName),
                new Claim(ClaimTypes.NameIdentifier, account.UserId.ToString()),
                new Claim(ClaimTypes.Role, account.Role)
            };

            // S?A L?I T?I ?ÂY: Dùng ?úng tên ?ã ??ng ký
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties();
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            switch (account.Role)
            {
                case "Admin": return RedirectToPage("/Admin/Index");
                case "Staff": return RedirectToPage("/Staff/Index");
                case "Technician": return RedirectToPage("/Technician/Jobs/Index");
                default: return RedirectToPage("/Customer/Index");
            }
        }
    }
}
