using EVCenterService.Data;
using EVCenterService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EVCenterService.Pages.Admin.Services
{
    [Authorize(Roles = "Admin")]
    public class DeleteModel : PageModel
    {
        private readonly EVServiceCenterContext _context;

        public DeleteModel(EVServiceCenterContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ServiceCatalog Service { get; set; } = default!;
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();
            var service = await _context.ServiceCatalogs.FirstOrDefaultAsync(m => m.ServiceId == id);
            if (service == null) return NotFound();

            Service = service;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null) return NotFound();

            var serviceToDelete = await _context.ServiceCatalogs.FindAsync(id);
            if (serviceToDelete == null) return NotFound();

            var isServiceInUse = await _context.OrderDetails.AnyAsync(od => od.ServiceId == id);
            if (isServiceInUse)
            {
                ErrorMessage = $"Kh�ng th? x�a d?ch v? '{serviceToDelete.Name}' v� n� ?� ???c s? d?ng trong c�c ??n h�ng. H�y xem x�t vi?c ch?nh s?a ho?c v� hi?u h�a d?ch v? n�y.";
                Service = serviceToDelete;
                return Page();
            }

            _context.ServiceCatalogs.Remove(serviceToDelete);
            await _context.SaveChangesAsync();

            TempData["StatusMessage"] = $"D?ch v? '{serviceToDelete.Name}' ?� ???c x�a th�nh c�ng.";
            return RedirectToPage("./Index");
        }
    }
}