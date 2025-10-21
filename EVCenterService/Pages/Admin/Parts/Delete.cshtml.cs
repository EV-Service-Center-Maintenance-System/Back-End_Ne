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

            // KI?M TRA RÀNG BU?C 1: Ph? tùng ?ã ???c s? d?ng ch?a?
            var isPartUsed = await _context.PartsUseds.AnyAsync(pu => pu.PartId == id);
            if (isPartUsed)
            {
                ErrorMessage = $"Không th? xóa ph? tùng '{partToDelete.Name}' vì nó ?ã ???c s? d?ng trong các l?n d?ch v? tr??c ?ây.";
                Part = partToDelete;
                return Page();
            }

            // KI?M TRA RÀNG BU?C 2: Ph? tùng còn t?n kho không? (Ki?m tra c? Storage)
            var isInStorage = await _context.Storages.AnyAsync(s => s.PartId == id && s.Quantity > 0);
            if (isInStorage)
            {
                ErrorMessage = $"Không th? xóa ph? tùng '{partToDelete.Name}' vì v?n còn t?n kho ({_context.Storages.Where(s => s.PartId == id).Sum(s => s.Quantity)} cái). Vui lòng ?i?u ch?nh kho v? 0 tr??c khi xóa.";
                Part = partToDelete;
                return Page();
            }

            // N?u không có ràng bu?c, ti?n hành xóa
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Xóa các b?n ghi trong Storage (n?u có, dù Quantity = 0)
                var storageEntries = await _context.Storages.Where(s => s.PartId == id).ToListAsync();
                if (storageEntries.Any())
                {
                    _context.Storages.RemoveRange(storageEntries);
                }

                // Xóa Part
                _context.Parts.Remove(partToDelete);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["StatusMessage"] = $"Ph? tùng '{partToDelete.Name}' ?ã ???c xóa thành công.";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                ErrorMessage = $"?ã x?y ra l?i khi xóa ph? tùng. Vui lòng th? l?i.";
                Part = partToDelete;
                return Page();
            }
        }
    }
}