using EVCenterService.Data;
using EVCenterService.Models;
using EVCenterService.Service.Interfaces; // <-- Thêm using IEmailSender
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity; // <-- Ch? dùng IPasswordHasher
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using AccountEntity = EVCenterService.Models.Account; // Gi? alias

namespace EVCenterService.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly EVServiceCenterContext _context;
        private readonly IPasswordHasher<AccountEntity> _passwordHasher; // Dùng l?i Hasher
        private readonly IEmailSender _emailSender; // <-- Thêm Mailjet sender

        public RegisterModel(
            EVServiceCenterContext context,
            IPasswordHasher<AccountEntity> passwordHasher,
            IEmailSender emailSender) // <-- Thêm vào constructor
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _emailSender = emailSender; // <-- Gán
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        // InputModel gi? nguyên validation
        public class InputModel
        { /* ... gi?ng file b?n cung c?p ... */
            [Required(ErrorMessage = "H? và tên là b?t bu?c.")][StringLength(100)] public string FullName { get; set; } = "";
            [Required(ErrorMessage = "Email là b?t bu?c.")][EmailAddress][StringLength(255)] public string Email { get; set; } = "";
            [Required(ErrorMessage = "S? ?i?n tho?i là b?t bu?c.")][RegularExpression(@"^(\+?84|0)\d{9,10}$", ErrorMessage = "S?T không h?p l?.")] public string PhoneNumber { get; set; } = "";
            [Required(ErrorMessage = "M?t kh?u là b?t bu?c.")][DataType(DataType.Password)][MinLength(6)][StringLength(100)] public string Password { get; set; } = "";
            [DataType(DataType.Password)][Compare("Password", ErrorMessage = "M?t kh?u không kh?p.")] public string ConfirmPassword { get; set; } = "";
        }


        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            // Ki?m tra Email và S?T t?n t?i
            if (await _context.Accounts.AnyAsync(a => a.Email == Input.Email))
            {
                ModelState.AddModelError("Input.Email", "??a ch? email này ?ã ???c s? d?ng.");
                return Page();
            }
            if (!string.IsNullOrEmpty(Input.PhoneNumber) && await _context.Accounts.AnyAsync(a => a.Phone == Input.PhoneNumber))
            {
                ModelState.AddModelError("Input.PhoneNumber", "S? ?i?n tho?i này ?ã ???c s? d?ng.");
                return Page();
            }

            // T?o tài kho?n m?i
            var newAccount = new AccountEntity
            {
                UserId = Guid.NewGuid(),
                FullName = Input.FullName,
                Email = Input.Email,
                Phone = Input.PhoneNumber,
                Role = "Customer", // M?c ??nh
                Status = "Active" // M?c ??nh Active (không c?n xác th?c email n?a)
            };
            // Hash m?t kh?u
            newAccount.Password = _passwordHasher.HashPassword(newAccount, Input.Password);

            _context.Accounts.Add(newAccount);
            await _context.SaveChangesAsync();

            // --- G?I EMAIL CHÀO M?NG (Dùng Mailjet) ---
            try
            {
                var subject = "Chào m?ng b?n ??n v?i EV Service Center!";
                var message = $"Chào {newAccount.FullName},<br><br>" +
                              "C?m ?n b?n ?ã ??ng ký tài kho?n thành công t?i EV Service Center.<br>" +
                              "Chúc b?n có nh?ng tr?i nghi?m d?ch v? t?t nh?t!<br><br>" +
                              "Trân tr?ng,<br>??i ng? EV Service Center";
                await _emailSender.SendEmailAsync(newAccount.Email, subject, message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"L?i g?i mail ??ng ký: {ex.Message}");
                // Không d?ng l?i n?u g?i mail l?i, ch? log
            }
            // --- K?T THÚC G?I MAIL ---

            // T? ??ng ??ng nh?p ng??i dùng sau khi ??ng ký
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, newAccount.Email),
                new Claim(ClaimTypes.GivenName, newAccount.FullName ?? string.Empty),
                new Claim(ClaimTypes.NameIdentifier, newAccount.UserId.ToString()),
                new Claim(ClaimTypes.Role, newAccount.Role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true, // Gi? ??ng nh?p
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            // Chuy?n h??ng ??n trang Customer Dashboard
            return RedirectToPage("/Customer/Index");
        }
    }
}