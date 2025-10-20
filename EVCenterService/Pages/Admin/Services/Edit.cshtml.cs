using EVCenterService.Data;
using EVCenterService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EVCenterService.Pages.Admin.Services
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly EVServiceCenterContext _context;

        public EditModel(EVServiceCenterContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ServiceCatalog Service { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service = await _context.ServiceCatalogs.FirstOrDefaultAsync(m => m.ServiceId == id);
            if (service == null)
            {
                return NotFound();
            }
            Service = service;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Service).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceExists(Service.ServiceId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            TempData["StatusMessage"] = $"D?ch v? '{Service.Name}' ?ã ???c c?p nh?t.";
            return RedirectToPage("./Index");
        }

        private bool ServiceExists(int id)
        {
            return _context.ServiceCatalogs.Any(e => e.ServiceId == id);
        }
    }
}
