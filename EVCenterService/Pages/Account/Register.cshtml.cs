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
            [Required(ErrorMessage = "Họ và tên là bắt buộc.")][StringLength(100)] public string FullName { get; set; } = "";
            [Required(ErrorMessage = "Email là bắt buộc.")][EmailAddress][StringLength(255)] public string Email { get; set; } = "";
            [Required(ErrorMessage = "Số điện thoại là bắt buộc.")][RegularExpression(@"^(\+?84|0)\d{9,10}$", ErrorMessage = "SĐT không hợp lệ.")] public string PhoneNumber { get; set; } = "";
            [Required(ErrorMessage = "Mật khẩu là bắt buộc.")][DataType(DataType.Password)][MinLength(6)][StringLength(100)] public string Password { get; set; } = "";
            [DataType(DataType.Password)][Compare("Password", ErrorMessage = "Mật khẩu không khớp.")] public string ConfirmPassword { get; set; } = "";
        }


        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            if (await _context.Accounts.AnyAsync(a => a.Email == Input.Email))
            {
                ModelState.AddModelError("Input.Email", "Địa chỉ email này đã được sử dụng.");
                return Page();
            }
            if (!string.IsNullOrEmpty(Input.PhoneNumber) && await _context.Accounts.AnyAsync(a => a.Phone == Input.PhoneNumber))
            {
                ModelState.AddModelError("Input.PhoneNumber", "Số điện thoại này đã được sử dụng.");
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

            try
            {
                var subject = "Chào mừng bạn đến với EV Service Center!";
                var message = $"Chào {newAccount.FullName},<br><br>" +
                              "Cảm ơn bạn đã đăng ký tài khoản thành công tại EV Service Center.<br>" +
                              "Chúc bạn có những trải nghiệm dịch vụ tốt nhất!<br><br>" +
                              "Trân trọng,<br> đội ngũ EV Service Center";
                await _emailSender.SendEmailAsync(newAccount.Email, subject, message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi gửi mail đăng ký: {ex.Message}");
            }

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
                IsPersistent = true, 
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return RedirectToPage("/Index");
        }
    }
}