using EVCenterService.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity; 
using AccountEntity = EVCenterService.Models.Account; 

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
        public InputModel Input { get; set; } = new();

        [BindProperty(SupportsGet = true)] 
        public string? ReturnUrl { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Vui lòng nhập Email.")]
            [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;
        }

        public async Task OnGetAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Email == Input.Email);

            if (account == null || string.IsNullOrEmpty(account.Password) || _passwordHasher.VerifyHashedPassword(account, account.Password, Input.Password) == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không đúng.");
                return Page();
            }

            if (account.Status != "Active")
            {
                ModelState.AddModelError(string.Empty, "Tài khoản của bạn đã bị khóa hoặc chưa kích hoạt.");
                return Page();
            }

            return await SignInUserAsync(account, ReturnUrl);
        }

        public IActionResult OnPostGoogleLogin()
        {
            var redirectUrl = Url.Page("./Login", pageHandler: "GoogleCallback", values: new { ReturnUrl });
            var properties = new AuthenticationProperties
            {
                RedirectUri = redirectUrl,
                Items = { { "prompt", "select_account" } }
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> OnGetGoogleCallbackAsync(string? remoteError = null)
        {
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Lỗi từ Google: {remoteError}");
                return Page();
            }

            var info = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if (info?.Principal == null)
            {
                info = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                if (info?.Principal == null)
                {
                    ModelState.AddModelError(string.Empty, "Không thể lấy thông tin đăng nhập từ Google.");
                    return Page();
                }
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var name = info.Principal.FindFirstValue(ClaimTypes.Name); 

            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError(string.Empty, "Không thể lấy địa chỉ email từ Google.");
                return Page();
            }

            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Email == email);

            if (account != null)
            {
                if (account.Status != "Active")
                {
                    ModelState.AddModelError(string.Empty, "Tài khoản Google này đã liên kết với một tài khoản bị khóa.");
                    return Page();
                }
                return await SignInUserAsync(account, ReturnUrl);
            }
            else
            {
                var newAccount = new AccountEntity
                {
                    UserId = Guid.NewGuid(),
                    FullName = name ?? email.Split('@')[0], 
                    Email = email,
                    Role = "Customer", 
                    Status = "Active",
                    Password = ""                
                };

                _context.Accounts.Add(newAccount);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError(string.Empty, "Lỗi khi tạo tài khoản. Email có thể đã tồn tại.");
                    Console.WriteLine($"Error creating Google user: {ex.InnerException?.Message ?? ex.Message}");
                    return Page();
                }

                return await SignInUserAsync(newAccount, ReturnUrl);
            }
        }

        private async Task<IActionResult> SignInUserAsync(AccountEntity account, string? returnUrl = null)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, account.Email),
                new Claim(ClaimTypes.GivenName, account.FullName ?? string.Empty),
                new Claim(ClaimTypes.NameIdentifier, account.UserId.ToString()),
                new Claim(ClaimTypes.Role, account.Role)
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

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                switch (account.Role)
                {
                    case "Admin": return RedirectToPage("/Admin/Index");
                    case "Staff": return RedirectToPage("/Staff/Index");
                    case "Technician": return RedirectToPage("/Technician/Jobs/Index");
                    default: return RedirectToPage("/Index"); 
                }
            }
        }
    }
}