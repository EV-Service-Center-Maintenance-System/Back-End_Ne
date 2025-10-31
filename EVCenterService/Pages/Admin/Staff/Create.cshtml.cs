using EVCenterService.Data;
using EVCenterService.Models;
using EVCenterService.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AccountEntity = EVCenterService.Models.Account;

namespace EVCenterService.Pages.Admin.Staff
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly IAdminEmployeeService _employeeService;

        public CreateModel(IAdminEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [BindProperty]
        public AccountEntity EmployeeAccount { get; set; } = new();

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            await _employeeService.CreateAsync(EmployeeAccount);

            TempData["StatusMessage"] = $"Tạo nhân viên '{EmployeeAccount.FullName}' thành công.";
            return RedirectToPage("./Index");
        }
    }
}
