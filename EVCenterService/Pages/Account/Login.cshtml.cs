using EVCenterService.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using EVCenterService.Service.Services;

namespace Test.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly EVServiceCenterContext _context;
        private readonly PasswordHasherService _passwordHasher;

        public LoginModel(EVServiceCenterContext context, PasswordHasherService passwordHasher)
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

            if (account == null || !_passwordHasher.Verify(Input.Password, account.Password))
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return Page();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, account.Email),
                new Claim(ClaimTypes.GivenName, account.FullName),
                new Claim(ClaimTypes.NameIdentifier, account.UserId.ToString()),
                new Claim(ClaimTypes.Role, account.Role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            switch (account.Role)
            {
                case "Admin": return RedirectToPage("/Admin/Index");
                case "Staff": return RedirectToPage("/Staff/Index");
                case "Technician": return RedirectToPage("/Technician/Index");
                default: return RedirectToPage("/Customer/Appointments/Index");
            }
        }
    }
}
