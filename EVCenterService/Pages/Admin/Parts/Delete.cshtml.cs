using EVCenterService.Data;
using EVCenterService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EVCenterService.Pages.Admin.Parts
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
        public Part Part { get; set; } = default!;
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            var part = await _context.Parts.FirstOrDefaultAsync(m => m.PartId == id);
            if (part == null) return NotFound();

            Part = part;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null) return NotFound();

            var partToDelete = await _context.Parts.FindAsync(id);
            if (partToDelete == null) return NotFound();

            // KI?M TRA R�NG BU?C 1: Ph? t�ng ?� ???c s? d?ng ch?a?
            var isPartUsed = await _context.PartsUseds.AnyAsync(pu => pu.PartId == id);
            if (isPartUsed)
            {
                ErrorMessage = $"Kh�ng th? x�a ph? t�ng '{partToDelete.Name}' v� n� ?� ???c s? d?ng trong c�c l?n d?ch v? tr??c ?�y.";
                Part = partToDelete;
                return Page();
            }

            // KI?M TRA R�NG BU?C 2: Ph? t�ng c�n t?n kho kh�ng? (Ki?m tra c? Storage)
            var isInStorage = await _context.Storages.AnyAsync(s => s.PartId == id && s.Quantity > 0);
            if (isInStorage)
            {
                ErrorMessage = $"Kh�ng th? x�a ph? t�ng '{partToDelete.Name}' v� v?n c�n t?n kho ({_context.Storages.Where(s => s.PartId == id).Sum(s => s.Quantity)} c�i). Vui l�ng ?i?u ch?nh kho v? 0 tr??c khi x�a.";
                Part = partToDelete;
                return Page();
            }

            // N?u kh�ng c� r�ng bu?c, ti?n h�nh x�a
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // X�a c�c b?n ghi trong Storage (n?u c�, d� Quantity = 0)
                var storageEntries = await _context.Storages.Where(s => s.PartId == id).ToListAsync();
                if (storageEntries.Any())
                {
                    _context.Storages.RemoveRange(storageEntries);
                }

                // X�a Part
                _context.Parts.Remove(partToDelete);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["StatusMessage"] = $"Ph? t�ng '{partToDelete.Name}' ?� ???c x�a th�nh c�ng.";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                ErrorMessage = $"?� x?y ra l?i khi x�a ph? t�ng. Vui l�ng th? l?i.";
                Part = partToDelete;
                return Page();
            }
        }
    }
}