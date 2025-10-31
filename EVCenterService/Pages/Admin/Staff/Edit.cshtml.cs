using EVCenterService.Data;
using EVCenterService.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AccountEntity = EVCenterService.Models.Account;

namespace EVCenterService.Pages.Admin.Staff
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly IAdminEmployeeService _employeeService;

        public EditModel(IAdminEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [BindProperty]
        public AccountEntity EmployeeAccount { get; set; } = new();

        [BindProperty]
        public string? NewPassword { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null) return NotFound();

            var employee = await _employeeService.GetByIdAsync(id.Value);
            if (employee == null) return NotFound();

            EmployeeAccount = employee;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            await _employeeService.UpdateAsync(EmployeeAccount, NewPassword);
            TempData["StatusMessage"] = $"Cập nhật thông tin nhân viên '{EmployeeAccount.FullName}' thành công.";
            return RedirectToPage("./Index");
        }
    }
}
