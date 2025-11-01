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
    public class DeleteModel : PageModel
    {
        private readonly IServiceCatalogService _service;

        public DeleteModel(IServiceCatalogService service)
        {
            _service = service;
        }

        [BindProperty]
        public ServiceCatalog Service { get; set; } = default!;

        public string ErrorMessage { get; set; } = string.Empty;

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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
                return NotFound();

            try
            {
                await _service.DeleteServiceAsync(id.Value);
                TempData["StatusMessage"] = $"🗑️ Dịch vụ '{Service.Name}' đã được xóa thành công.";
                return RedirectToPage("./Index");
            }
            catch (InvalidOperationException ex)
            {
                ErrorMessage = ex.Message;
                var serviceItem = await _service.GetServiceByIdAsync(id.Value);
                if (serviceItem != null) Service = serviceItem;
                return Page();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}