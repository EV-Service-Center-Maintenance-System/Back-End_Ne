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

        public ProfileModel(EVServiceCenterContext context)
        {
            _context = context;
        }

        public string Email { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Họ và tên là bắt buộc.")]
            [Display(Name = "Họ và tên")]
            public string FullName { get; set; }

            [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
            [RegularExpression(@"^(\+?84|0)\d{9,10}$", ErrorMessage = "Số điện thoại không hợp lệ.")]
            [Display(Name = "Số điện thoại")]
            public string PhoneNumber { get; set; }
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

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _context.Accounts.FindAsync(Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)));
            if (user == null) return Page();

            if (!ModelState.IsValid)
            {
                await LoadUserDataAsync();
                return Page();
            }

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
    }
}