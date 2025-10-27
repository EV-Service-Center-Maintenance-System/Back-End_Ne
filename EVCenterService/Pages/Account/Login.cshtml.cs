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
        private readonly IPasswordHasher<AccountEntity> _passwordHasher; // D�ng l?i Hasher

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
            [Required(ErrorMessage = "Vui l�ng nh?p Email.")]
            [EmailAddress(ErrorMessage = "Email kh�ng h?p l?.")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Vui l�ng nh?p m?t kh?u.")]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;
        }

        public async Task OnGetAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;
            // X�a cookie b�n ngo�i n?u c� (?? ??m b?o login Google s?ch)
            //await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
        }

        // --- HANDLER ??NG NH?P TH??NG ---
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Email == Input.Email);

            // Ki?m tra t�i kho?n v� m?t kh?u
            if (account == null || string.IsNullOrEmpty(account.Password) || _passwordHasher.VerifyHashedPassword(account, account.Password, Input.Password) == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(string.Empty, "Email ho?c m?t kh?u kh�ng ?�ng.");
                return Page();
            }

            // Ki?m tra tr?ng th�i t�i kho?n (Th�m ki?m tra n�y)
            if (account.Status != "Active")
            {
                ModelState.AddModelError(string.Empty, "T�i kho?n c?a b?n ?� b? kh�a ho?c ch?a k�ch ho?t.");
                return Page();
            }

            // D�ng h�m helper ?? ??ng nh?p
            return await SignInUserAsync(account, ReturnUrl);
        }

        // --- HANDLER CHO N�T GOOGLE ---
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

        // --- HANDLER X? L� CALLBACK T? GOOGLE ---
        public async Task<IActionResult> OnGetGoogleCallbackAsync(string? remoteError = null)
        {
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"L?i t? Google: {remoteError}");
                return Page();
            }

            // L?y th�ng tin t? cookie Google tr? v?
            var info = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if (info?.Principal == null)
            {
                // Th? l?y t? cookie ch�nh n?u Google scheme kh�ng c�
                info = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                if (info?.Principal == null)
                {
                    ModelState.AddModelError(string.Empty, "Kh�ng th? l?y th�ng tin ??ng nh?p t? Google.");
                    return Page();
                }
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var name = info.Principal.FindFirstValue(ClaimTypes.Name); 

            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError(string.Empty, "Kh�ng th? l?y ??a ch? email t? Google.");
                return Page();
            }

            // T�m t�i kho?n trong DB
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Email == email);

            // ??ng xu?t kh?i cookie t?m th?i c?a Google
            //await HttpContext.SignOutAsync(GoogleDefaults.AuthenticationScheme);

            if (account != null)
            {
                // T�i kho?n t?n t?i, ki?m tra tr?ng th�i r?i ??ng nh?p
                if (account.Status != "Active")
                {
                    ModelState.AddModelError(string.Empty, "T�i kho?n Google n�y ?� li�n k?t v?i m?t t�i kho?n b? kh�a.");
                    return Page();
                }
                return await SignInUserAsync(account, ReturnUrl);
            }
            else
            {
                // T�i kho?n ch?a t?n t?i -> T? ??ng t?o t�i kho?n Customer m?i
                var newAccount = new AccountEntity
                {
                    UserId = Guid.NewGuid(),
                    FullName = name ?? email.Split('@')[0], // L?y t�n n?u c�, n?u kh�ng th� l?y ph?n tr??c @
                    Email = email,
                    // Kh�ng l?y S?T t? Google
                    Role = "Customer", // M?c ??nh
                    Status = "Active",
                    Password = "" // Quan tr?ng: ??t m?t kh?u r?ng ho?c m?t gi� tr? ??c bi?t
                                  // v� kh�ng c� m?t kh?u khi ??ng nh?p b?ng Google
                                  // Kh�ng c?n hash
                };

                _context.Accounts.Add(newAccount);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError(string.Empty, "L?i khi t?o t�i kho?n. Email c� th? ?� t?n t?i.");
                    Console.WriteLine($"Error creating Google user: {ex.InnerException?.Message ?? ex.Message}");
                    return Page();
                }

                // ??ng nh?p t�i kho?n v?a t?o
                return await SignInUserAsync(newAccount, ReturnUrl);
            }
        }
        // --- K?T TH�C HANDLER GOOGLE ---

        // --- H�M HELPER ??NG NH?P (D�ng chung) ---
        private async Task<IActionResult> SignInUserAsync(AccountEntity account, string? returnUrl = null)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, account.Email),
                new Claim(ClaimTypes.GivenName, account.FullName ?? string.Empty),
                new Claim(ClaimTypes.NameIdentifier, account.UserId.ToString()),
                new Claim(ClaimTypes.Role, account.Role)
                // Th�m c�c Claim kh�c n?u c?n (v� d?: S? ?i?n tho?i)
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
                    default: return RedirectToPage("/Customer/Index"); // M?c ??nh l� Customer
                }
            }
        }
        // --- K?T TH�C HELPER ---
    }
}