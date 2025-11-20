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

            var isPartUsed = await _context.PartsUseds.AnyAsync(pu => pu.PartId == id);
            if (isPartUsed)
            {
                ErrorMessage = $"Không thể xóa phụ tùng '{partToDelete.Name}' vì nó đã được sử dụng trong các lần dịch vụ trước đây.";
                Part = partToDelete;
                return Page();
            }

            var isInStorage = await _context.Storages.AnyAsync(s => s.PartId == id && s.Quantity > 0);
            if (isInStorage)
            {
                ErrorMessage = $"Không thể xóa phụ tùng '{partToDelete.Name}' vì vẫn còn tồn kho ({_context.Storages.Where(s => s.PartId == id).Sum(s => s.Quantity)} cái). Vui lòng điều chỉnh kho về 0 trước khi xóa.";
                Part = partToDelete;
                return Page();
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var storageEntries = await _context.Storages.Where(s => s.PartId == id).ToListAsync();
                if (storageEntries.Any())
                {
                    _context.Storages.RemoveRange(storageEntries);
                }

                // Xóa Part
                _context.Parts.Remove(partToDelete);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["StatusMessage"] = $"Phụ tùng '{partToDelete.Name}' đã được xóa thành công.";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                ErrorMessage = $"Đã xảy ra lỗi khi xóa phụ tùng. Vui lòng thử lại.";
                Part = partToDelete;
                return Page();
            }
        }
    }
}