using EVCenterService.Data;
using EVCenterService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;

// Sửa lỗi xung đột namespace
using AccountEntity = EVCenterService.Models.Account;

namespace EVCenterService.Pages.Customer
{
    [Authorize(Roles = "Customer")]
    public class ChangePasswordModel : PageModel
    {
        private readonly EVServiceCenterContext _context;
        private readonly IPasswordHasher<AccountEntity> _passwordHasher;

        public ChangePasswordModel(
            EVServiceCenterContext context,
            IPasswordHasher<AccountEntity> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public PasswordModel PasswordInput { get; set; }

        public class PasswordModel
        {
            [Required(ErrorMessage = "Mật khẩu cũ là bắt buộc.")]
            [DataType(DataType.Password)]
            [Display(Name = "Mật khẩu cũ")]
            public string OldPassword { get; set; }

            [Required(ErrorMessage = "Mật khẩu mới là bắt buộc.")]
            [StringLength(100, ErrorMessage = "{0} phải dài ít nhất {2} ký tự.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Mật khẩu mới")]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Xác nhận mật khẩu mới")]
            [Compare("NewPassword", ErrorMessage = "Mật khẩu mới và mật khẩu xác nhận không khớp.")]
            public string ConfirmPassword { get; set; }
        }

        public void OnGet()
        {
            // Chỉ hiển thị trang
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _context.Accounts.FindAsync(Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)));
            if (user == null) return Page();

            // Chỉ validate form này
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, PasswordInput.OldPassword);
            if (verificationResult == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError("PasswordInput.OldPassword", "Mật khẩu cũ không chính xác.");
                return Page();
            }

            user.Password = _passwordHasher.HashPassword(user, PasswordInput.NewPassword);
            _context.Accounts.Update(user);
            await _context.SaveChangesAsync();

            StatusMessage = "Mật khẩu của bạn đã được thay đổi thành công.";
            return RedirectToPage(); // Tải lại trang này (ChangePassword) và hiển thị thông báo
        }
    }
}