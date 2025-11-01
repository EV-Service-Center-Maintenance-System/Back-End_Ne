using EVCenterService.Data;
using EVCenterService.Models;
using EVCenterService.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EVCenterService.Pages.Admin.Services
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly IServiceCatalogService _service;

        public EditModel(IServiceCatalogService service)
        {
            _service = service;
        }

        [BindProperty]
        public ServiceCatalog Service { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            var serviceItem = await _service.GetServiceByIdAsync(id.Value);
            if (serviceItem == null)
                return NotFound();

            Service = serviceItem;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            try
            {
                await _service.UpdateServiceAsync(Service);
                TempData["StatusMessage"] = $"✅ Dịch vụ '{Service.Name}' đã được cập nhật thành công.";
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi: {ex.Message}");
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}
