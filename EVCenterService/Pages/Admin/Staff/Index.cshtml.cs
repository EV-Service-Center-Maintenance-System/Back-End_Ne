using EVCenterService.Data;
using EVCenterService.Models;
using EVCenterService.Models.Enum;
using EVCenterService.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AccountEntity = EVCenterService.Models.Account;

namespace EVCenterService.Pages.Admin.Staff
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly IAdminEmployeeService _employeeService;

        public IndexModel(IAdminEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public IList<AccountEntity> EmployeeList { get; set; } = new List<AccountEntity>();

        public async Task OnGetAsync()
        {
            EmployeeList = await _employeeService.GetAllAsync();
        }

        public async Task<IActionResult> OnPostToggleStatusAsync(Guid id)
        {
            await _employeeService.ToggleStatusAsync(id);
            TempData["StatusMessage"] = "Cập nhật trạng thái tài khoản thành công.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(Guid id)
        {
            await _employeeService.DeleteAsync(id);
            TempData["StatusMessage"] = "Đã xóa nhân viên khỏi hệ thống.";
            return RedirectToPage();
        }
    }
}
