using EVCenterService.Data;
using EVCenterService.Models;
using EVCenterService.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EVCenterService.Pages.Admin.Services
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly IServiceCatalogService _service;

        public CreateModel(IServiceCatalogService service)
        {
            _service = service;
        }

        [BindProperty]
        public ServiceCatalog Service { get; set; } = new();

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            await _service.CreateServiceAsync(Service);

            TempData["StatusMessage"] = $"✅ Dịch vụ '{Service.Name}' đã được tạo thành công.";
            return RedirectToPage("./Index");
        }
    }
}
