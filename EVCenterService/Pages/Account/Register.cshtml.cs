using EVCenterService.Data;
using System.Security.Claims;
using EVCenterService.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using EVCenterService.Service.Services;


namespace Test.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly EVServiceCenterContext _context;
        private readonly PasswordHasherService _passwordHasher;

        // ?? Dependency Injection: T? ??ng n?p DbContext v� PasswordHasherService
        public RegisterModel(EVServiceCenterContext context, PasswordHasherService passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        // ?? [BindProperty]: T? ??ng li�n k?t d? li?u t? form HTML v�o ??i t??ng Input
        [BindProperty]
        public InputModel Input { get; set; }

        // L?p ch?a c�c thu?c t�nh c?a form v� c�c quy t?c x�c th?c (validation)
        public class InputModel
        {
            [Required(ErrorMessage = "Full name is required.")]
            public string FullName { get; set; }

            [Required(ErrorMessage = "Email is required.")]
            [EmailAddress(ErrorMessage = "Invalid email format.")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Phone number is required.")]
            public string PhoneNumber { get; set; }

            [Required(ErrorMessage = "Password is required.")]
            [DataType(DataType.Password)]
            [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
            public string Password { get; set; }

            [Required(ErrorMessage = "Please confirm your password.")]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public void OnGet()
        {
            // H�m n�y ???c g?i khi trang ???c t?i l?n ??u (GET request)
        }

        // ?? H�m n�y ???c g?i khi ng??i d�ng nh?n n�t "Create Account" (POST request)
        public async Task<IActionResult> OnPostAsync()
        {
            // N?u d? li?u form kh�ng h?p l? (v� d?: thi?u email, m?t kh?u kh�ng kh?p), hi?n th? l?i form v?i th�ng b�o l?i
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Ki?m tra xem email ?� c� ng??i s? d?ng ch?a
            if (await _context.Accounts.AnyAsync(a => a.Email == Input.Email))
            {
                ModelState.AddModelError(string.Empty, "An account with this email already exists.");
                return Page();
            }

            // T?o m?t ??i t??ng Account m?i t? d? li?u ng??i d�ng nh?p
            var newAccount = new EVCenterService.Models.Account
            {
                UserId = Guid.NewGuid(),
                FullName = Input.FullName,
                Email = Input.Email,
                Phone = Input.PhoneNumber,
                Password = _passwordHasher.Hash(Input.Password),
                Role = "Customer"
            };

            // Th�m t�i kho?n m?i v�o DbContext v� l?u v�o database
            _context.Accounts.Add(newAccount);
            await _context.SaveChangesAsync();

            // T? ??ng ??ng nh?p ng??i d�ng ngay sau khi ??ng k� th�nh c�ng
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, newAccount.Email),
                new Claim(ClaimTypes.GivenName, newAccount.FullName),
                new Claim(ClaimTypes.NameIdentifier, newAccount.UserId.ToString()),
                new Claim(ClaimTypes.Role, newAccount.Role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            // Chuy?n h??ng ng??i d�ng ??n trang dashboard c?a h?
            return RedirectToPage("/Customer/Appointments/Index");
        }
    }
}
