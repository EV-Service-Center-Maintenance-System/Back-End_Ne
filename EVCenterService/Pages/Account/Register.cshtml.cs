using EVCenterService.Data;
using EVCenterService.Models; 
using EVCenterService.Service; 
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using AccountEntity = EVCenterService.Models.Account;
using Microsoft.AspNetCore.Authentication.Cookies;


namespace Test.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly EVServiceCenterContext _context;
        private readonly IPasswordHasher<AccountEntity> _passwordHasher;

        public RegisterModel(EVServiceCenterContext context, IPasswordHasher<AccountEntity> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        // ... (class InputModel gi? nguy�n)
        public class InputModel
        {
            [Required(ErrorMessage = "H? v� t�n l� b?t bu?c.")]
            public string FullName { get; set; }
            [Required(ErrorMessage = "Email l� b?t bu?c.")]
            [EmailAddress(ErrorMessage = "??nh d?ng email kh�ng h?p l?.")]
            public string Email { get; set; }
            [Required(ErrorMessage = "S? ?i?n tho?i l� b?t bu?c.")]
            public string PhoneNumber { get; set; }
            [Required(ErrorMessage = "M?t kh?u l� b?t bu?c.")]
            [DataType(DataType.Password)]
            [MinLength(6, ErrorMessage = "M?t kh?u ph?i c� �t nh?t 6 k� t?.")]
            public string Password { get; set; }
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "M?t kh?u v� m?t kh?u x�c nh?n kh�ng kh?p.")]
            public string ConfirmPassword { get; set; }
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            if (await _context.Accounts.AnyAsync(a => a.Email == Input.Email))
            {
                ModelState.AddModelError(string.Empty, "M?t t�i kho?n v?i email n�y ?� t?n t?i.");
                return Page();
            }

            var newAccount = new AccountEntity
            {
                UserId = Guid.NewGuid(),
                FullName = Input.FullName,
                Email = Input.Email,
                Phone = Input.PhoneNumber,
                Role = "Customer",
                Status = "Active"
            };
            newAccount.Password = _passwordHasher.HashPassword(newAccount, Input.Password);

            _context.Accounts.Add(newAccount);
            await _context.SaveChangesAsync();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, newAccount.Email),
                new Claim(ClaimTypes.GivenName, newAccount.FullName),
                new Claim(ClaimTypes.NameIdentifier, newAccount.UserId.ToString()),
                new Claim(ClaimTypes.Role, newAccount.Role)
            };

            // S?A L?I T?I ?�Y: D�ng ?�ng t�n ?� ??ng k�
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            return RedirectToPage("/Customer/Appointments/Index");
        }
    }
}
