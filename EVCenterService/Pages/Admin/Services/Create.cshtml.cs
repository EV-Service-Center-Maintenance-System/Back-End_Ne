using EVCenterService.Data;
using EVCenterService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EVCenterService.Pages.Admin.Services
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly EVServiceCenterContext _context;

        public CreateModel(EVServiceCenterContext context)
        {
            _context = context;
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
            {
                return Page();
            }

            _context.ServiceCatalogs.Add(Service);
            await _context.SaveChangesAsync();

            TempData["StatusMessage"] = $"D?ch v? '{Service.Name}' ?ã ???c t?o thành công.";
            return RedirectToPage("./Index");
        }
    }
}
