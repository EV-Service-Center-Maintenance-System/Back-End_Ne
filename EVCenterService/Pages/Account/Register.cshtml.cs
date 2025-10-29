using EVCenterService.Data;
using EVCenterService.Models;
using EVCenterService.Service.Interfaces; 
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using AccountEntity = EVCenterService.Models.Account; 

namespace EVCenterService.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly EVServiceCenterContext _context;
        private readonly IPasswordHasher<AccountEntity> _passwordHasher; 
        private readonly IEmailSender _emailSender; 

        public RegisterModel(
            EVServiceCenterContext context,
            IPasswordHasher<AccountEntity> passwordHasher,
            IEmailSender emailSender) 
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _emailSender = emailSender; 
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        { 
            [Required(ErrorMessage = "H? v� t�n l� b?t bu?c.")][StringLength(100)] public string FullName { get; set; } = "";
            [Required(ErrorMessage = "Email l� b?t bu?c.")][EmailAddress][StringLength(255)] public string Email { get; set; } = "";
            [Required(ErrorMessage = "S? ?i?n tho?i l� b?t bu?c.")][RegularExpression(@"^(\+?84|0)\d{9,10}$", ErrorMessage = "S?T kh�ng h?p l?.")] public string PhoneNumber { get; set; } = "";
            [Required(ErrorMessage = "M?t kh?u l� b?t bu?c.")][DataType(DataType.Password)][MinLength(6)][StringLength(100)] public string Password { get; set; } = "";
            [DataType(DataType.Password)][Compare("Password", ErrorMessage = "M?t kh?u kh�ng kh?p.")] public string ConfirmPassword { get; set; } = "";
        }


        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            // Ki?m tra Email v� S?T t?n t?i
            if (await _context.Accounts.AnyAsync(a => a.Email == Input.Email))
            {
                ModelState.AddModelError("Input.Email", "??a ch? email n�y ?� ???c s? d?ng.");
                return Page();
            }
            if (!string.IsNullOrEmpty(Input.PhoneNumber) && await _context.Accounts.AnyAsync(a => a.Phone == Input.PhoneNumber))
            {
                ModelState.AddModelError("Input.PhoneNumber", "S? ?i?n tho?i n�y ?� ???c s? d?ng.");
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

            // --- G?I EMAIL CH�O M?NG (D�ng Mailjet) ---
            try
            {
                var subject = "Ch�o m?ng b?n ??n v?i EV Service Center!";
                var message = $"Ch�o {newAccount.FullName},<br><br>" +
                              "C?m ?n b?n ?� ??ng k� t�i kho?n th�nh c�ng t?i EV Service Center.<br>" +
                              "Ch�c b?n c� nh?ng tr?i nghi?m d?ch v? t?t nh?t!<br><br>" +
                              "Tr�n tr?ng,<br> EV Service Center";
                await _emailSender.SendEmailAsync(newAccount.Email, subject, message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"L?i g?i mail ??ng k�: {ex.Message}");
                // Kh�ng d?ng l?i n?u g?i mail l?i, ch? log
            }
            // --- K?T TH�C G?I MAIL ---

            // T? ??ng ??ng nh?p ng??i d�ng sau khi ??ng k�
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