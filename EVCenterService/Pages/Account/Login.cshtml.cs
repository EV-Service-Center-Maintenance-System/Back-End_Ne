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
        private readonly IPasswordHasher<AccountEntity> _passwordHasher; // Dùng l?i Hasher

        public LoginModel(EVServiceCenterContext context, IPasswordHasher<AccountEntity> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        [BindProperty(SupportsGet = true)] // L?y returnUrl t? query string
        public string? ReturnUrl { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Vui lòng nh?p Email.")]
            [EmailAddress(ErrorMessage = "Email không h?p l?.")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Vui lòng nh?p m?t kh?u.")]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;
        }

        public async Task OnGetAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;
            // Xóa cookie bên ngoài n?u có (?? ??m b?o login Google s?ch)
            //await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
        }

        // --- HANDLER ??NG NH?P TH??NG ---
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Email == Input.Email);

            // Ki?m tra tài kho?n và m?t kh?u
            if (account == null || string.IsNullOrEmpty(account.Password) || _passwordHasher.VerifyHashedPassword(account, account.Password, Input.Password) == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(string.Empty, "Email ho?c m?t kh?u không ?úng.");
                return Page();
            }

            // Ki?m tra tr?ng thái tài kho?n (Thêm ki?m tra này)
            if (account.Status != "Active")
            {
                ModelState.AddModelError(string.Empty, "Tài kho?n c?a b?n ?ã b? khóa ho?c ch?a kích ho?t.");
                return Page();
            }

            // Dùng hàm helper ?? ??ng nh?p
            return await SignInUserAsync(account, ReturnUrl);
        }

        // --- HANDLER CHO NÚT GOOGLE ---
        public IActionResult OnPostGoogleLogin()
        {
            // ???ng d?n callback s? g?i OnGetGoogleCallbackAsync
            var redirectUrl = Url.Page("./Login", pageHandler: "GoogleCallback", values: new { ReturnUrl });
            var properties = new AuthenticationProperties
            {
                RedirectUri = redirectUrl,
                Items = { { "prompt", "select_account" } }
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        // --- HANDLER X? LÝ CALLBACK T? GOOGLE ---
        public async Task<IActionResult> OnGetGoogleCallbackAsync(string? remoteError = null)
        {
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"L?i t? Google: {remoteError}");
                return Page();
            }

            // L?y thông tin t? cookie Google tr? v?
            var info = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if (info?.Principal == null)
            {
                // Th? l?y t? cookie chính n?u Google scheme không có
                info = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                if (info?.Principal == null)
                {
                    ModelState.AddModelError(string.Empty, "Không th? l?y thông tin ??ng nh?p t? Google.");
                    return Page();
                }
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var name = info.Principal.FindFirstValue(ClaimTypes.Name); 

            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError(string.Empty, "Không th? l?y ??a ch? email t? Google.");
                return Page();
            }

            // Tìm tài kho?n trong DB
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Email == email);

            // ??ng xu?t kh?i cookie t?m th?i c?a Google
            //await HttpContext.SignOutAsync(GoogleDefaults.AuthenticationScheme);

            if (account != null)
            {
                // Tài kho?n t?n t?i, ki?m tra tr?ng thái r?i ??ng nh?p
                if (account.Status != "Active")
                {
                    ModelState.AddModelError(string.Empty, "Tài kho?n Google này ?ã liên k?t v?i m?t tài kho?n b? khóa.");
                    return Page();
                }
                return await SignInUserAsync(account, ReturnUrl);
            }
            else
            {
                // Tài kho?n ch?a t?n t?i -> T? ??ng t?o tài kho?n Customer m?i
                var newAccount = new AccountEntity
                {
                    UserId = Guid.NewGuid(),
                    FullName = name ?? email.Split('@')[0], // L?y tên n?u có, n?u không thì l?y ph?n tr??c @
                    Email = email,
                    // Không l?y S?T t? Google
                    Role = "Customer", // M?c ??nh
                    Status = "Active",
                    Password = "" // Quan tr?ng: ??t m?t kh?u r?ng ho?c m?t giá tr? ??c bi?t
                                  // vì không có m?t kh?u khi ??ng nh?p b?ng Google
                                  // Không c?n hash
                };

                _context.Accounts.Add(newAccount);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError(string.Empty, "L?i khi t?o tài kho?n. Email có th? ?ã t?n t?i.");
                    Console.WriteLine($"Error creating Google user: {ex.InnerException?.Message ?? ex.Message}");
                    return Page();
                }

                // ??ng nh?p tài kho?n v?a t?o
                return await SignInUserAsync(newAccount, ReturnUrl);
            }
        }
        // --- K?T THÚC HANDLER GOOGLE ---

        // --- HÀM HELPER ??NG NH?P (Dùng chung) ---
        private async Task<IActionResult> SignInUserAsync(AccountEntity account, string? returnUrl = null)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, account.Email),
                new Claim(ClaimTypes.GivenName, account.FullName ?? string.Empty),
                new Claim(ClaimTypes.NameIdentifier, account.UserId.ToString()),
                new Claim(ClaimTypes.Role, account.Role)
                // Thêm các Claim khác n?u c?n (ví d?: S? ?i?n tho?i)
                // new Claim(ClaimTypes.MobilePhone, account.Phone ?? string.Empty)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true, // Gi? ??ng nh?p
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30)
            };

            // Th?c hi?n ??ng nh?p, t?o cookie
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            // Chuy?n h??ng
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
        // --- K?T THÚC HELPER ---
    }
}