using EVCenterService.Data;
using EVCenterService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;

// Sửa lỗi xung đột namespace (Giống file Register)
using AccountEntity = EVCenterService.Models.Account;

namespace EVCenterService.Pages.Customer
{
    [Authorize(Roles = "Customer")]
    public class ProfileModel : PageModel
    {
        private readonly EVServiceCenterContext _context;
        private readonly IPasswordHasher<AccountEntity> _passwordHasher;

        public ProfileModel(
            EVServiceCenterContext context,
            IPasswordHasher<AccountEntity> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public string Email { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        [BindProperty]
        public PasswordModel PasswordInput { get; set; }

        public class InputModel
        {
            // SỬA LỖI: Thêm [Required]
            [Required(ErrorMessage = "Họ và tên là bắt buộc.")]
            [Display(Name = "Họ và tên")]
            public string FullName { get; set; }

            // SỬA LỖI: Thêm [Required] và dùng Regex từ trang Register
            [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
            [RegularExpression(@"^(\+?84|0)\d{9,10}$", ErrorMessage = "Số điện thoại không hợp lệ.")]
            [Display(Name = "Số điện thoại")]
            public string PhoneNumber { get; set; }
        }

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

        // Tải dữ liệu người dùng
        private async Task LoadUserDataAsync()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Accounts.FindAsync(Guid.Parse(userIdString));

            Email = user.Email;
            Input = new InputModel
            {
                FullName = user.FullName,
                PhoneNumber = user.Phone
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || await _context.Accounts.FindAsync(Guid.Parse(userIdString)) == null)
            {
                return NotFound("Không tìm thấy người dùng.");
            }
            await LoadUserDataAsync();
            return Page();
        }

        // Xử lý khi nhấn "Lưu Thay đổi Thông tin"
        public async Task<IActionResult> OnPostUpdateInfoAsync()
        {
            var user = await _context.Accounts.FindAsync(Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)));
            if (user == null) return Page();

            // SỬA LỖI MẤU CHỐT: Xóa validation của form Mật khẩu
            // vì chúng ta chỉ submit form Thông tin
            ModelState.Remove(nameof(PasswordInput));
            ModelState.Remove("PasswordInput.OldPassword");
            ModelState.Remove("PasswordInput.NewPassword");
            ModelState.Remove("PasswordInput.ConfirmPassword");


            // Bây giờ, IsValid chỉ kiểm tra [Required] của InputModel
            if (!ModelState.IsValid)
            {
                // Nếu SĐT sai, lỗi sẽ hiển thị ở đây
                await LoadUserDataAsync();
                return Page();
            }

            // Kiểm tra trùng lặp SĐT (trừ SĐT của chính mình)
            if (await _context.Accounts.AnyAsync(a => a.Phone == Input.PhoneNumber && a.UserId != user.UserId))
            {
                ModelState.AddModelError("Input.PhoneNumber", "Số điện thoại này đã được sử dụng bởi tài khoản khác.");
                await LoadUserDataAsync();
                return Page();
            }

            user.FullName = Input.FullName;
            user.Phone = Input.PhoneNumber;

            _context.Accounts.Update(user);
            await _context.SaveChangesAsync();

            StatusMessage = "Thông tin cá nhân của bạn đã được cập nhật.";
            return RedirectToPage();
        }

        // Xử lý khi nhấn "Thay đổi Mật khẩu"
        public async Task<IActionResult> OnPostChangePasswordAsync()
        {
            var user = await _context.Accounts.FindAsync(Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)));
            if (user == null) return Page();

            // SỬA LỖI MẤU CHỐT: Xóa validation của form Thông tin
            // vì chúng ta chỉ submit form Mật khẩu
            ModelState.Remove(nameof(Input));
            ModelState.Remove("Input.FullName");
            ModelState.Remove("Input.PhoneNumber");

            // Bây giờ, IsValid chỉ kiểm tra [Required] của PasswordModel
            if (!ModelState.IsValid)
            {
                await LoadUserDataAsync();
                return Page();
            }

            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, PasswordInput.OldPassword);
            if (verificationResult == PasswordVerificationResult.Failed)
            {
                // Đây là lý do trang "load lại". Giờ nó sẽ hiện lỗi.
                ModelState.AddModelError("PasswordInput.OldPassword", "Mật khẩu cũ không chính xác.");
                await LoadUserDataAsync();
                return Page();
            }

            user.Password = _passwordHasher.HashPassword(user, PasswordInput.NewPassword);
            _context.Accounts.Update(user);
            await _context.SaveChangesAsync();

            StatusMessage = "Mật khẩu của bạn đã được thay đổi thành công.";
            return RedirectToPage();
        }
    }
}