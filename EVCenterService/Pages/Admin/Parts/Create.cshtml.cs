using EVCenterService.Data;
using EVCenterService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EVCenterService.Pages.Admin.Parts
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
        public Part Part { get; set; } = new();

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Ki?m tra xem PartId ?ã t?n t?i ch?a
            if (await _context.Parts.AnyAsync(p => p.PartId == Part.PartId))
            {
                ModelState.AddModelError("Part.PartId", "Mã Ph? tùng (ID) này ?ã t?n t?i.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // B?t ??u transaction
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Thêm Part m?i
                _context.Parts.Add(Part);
                await _context.SaveChangesAsync(); // L?u Part tr??c

                // 2. L?y danh sách t?t c? các CenterID
                var centerIds = await _context.MaintenanceCenters.Select(c => c.CenterId).ToListAsync();

                // 3. L?y StorageID l?n nh?t hi?n có ?? t?o ID m?i
                int maxStorageId = await _context.Storages.AnyAsync() ? await _context.Storages.MaxAsync(s => s.StorageId) : 0;
                int nextStorageId = maxStorageId + 1;

                // 4. T?o các b?n ghi Storage t??ng ?ng
                foreach (var centerId in centerIds)
                {
                    var storageEntry = new Storage
                    {
                        StorageId = nextStorageId++, // S?A L?I: Gán StorageID duy nh?t
                        CenterId = centerId,
                        PartId = Part.PartId,
                        Quantity = 0,
                        MinThreshold = 5
                    };
                    _context.Storages.Add(storageEntry);
                }
                await _context.SaveChangesAsync(); // L?u Storage

                // Commit transaction
                await transaction.CommitAsync();

                TempData["StatusMessage"] = $"Ph? tùng '{Part.Name}' ?ã ???c t?o và thêm vào kho các trung tâm.";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                // Rollback n?u có l?i
                await transaction.RollbackAsync();
                Console.WriteLine($"Error creating part: {ex.Message}"); 
                ModelState.AddModelError(string.Empty, "?ã x?y ra l?i khi t?o ph? tùng. Vui lòng th? l?i ho?c liên h? qu?n tr? viên.");
                return Page();
            }
        }
    }
}