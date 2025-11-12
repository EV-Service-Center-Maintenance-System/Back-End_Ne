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
            //// Ki?m tra xem PartId ?ã t?n t?i ch?a (vì PartId KHÔNG t? t?ng trong DB c?a b?n)
            //if (await _context.Parts.AnyAsync(p => p.PartId == Part.PartId))
            //{
            //    ModelState.AddModelError("Part.PartId", "Mã Ph? tùng (ID) này ?ã t?n t?i.");
            //}

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
                // L?u Part tr??c ?? ??m b?o PartId h?p l? cho b??c sau
                // (Quan tr?ng vì PartId không t? t?ng, b?n ph?i cung c?p nó t? form)
                await _context.SaveChangesAsync();

                // 2. L?y danh sách t?t c? các CenterID
                var centerIds = await _context.MaintenanceCenters.Select(c => c.CenterId).ToListAsync();

                // 3. T?o các b?n ghi Storage t??ng ?ng (KHÔNG c?n tính toán StorageID)
                foreach (var centerId in centerIds)
                {
                    var storageEntry = new Storage
                    {
                        // StorageId = nextStorageId++, // <-- XÓA DÒNG NÀY VÀ CÁC DÒNG TÍNH TOÁN ID ? TRÊN
                        CenterId = centerId,
                        PartId = Part.PartId, // PartId l?y t? ??i t??ng Part v?a ???c thêm
                        Quantity = 0, // S? l??ng ban ??u
                        MinThreshold = 5 // Ng??ng m?c ??nh
                    };
                    _context.Storages.Add(storageEntry);
                }
                // L?u các b?n ghi Storage (database s? t? t?o StorageID)
                await _context.SaveChangesAsync();

                // N?u m?i th? thành công, commit transaction
                await transaction.CommitAsync();

                TempData["StatusMessage"] = $"Phụ tùng '{Part.Name}' Đã được tạo và thêm vào kho các trung tâm.";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                // N?u có l?i, rollback transaction
                await transaction.RollbackAsync();
                Console.WriteLine($"Error creating part: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi tạo phụ tùng. Vui lòng thử lại hoặc liên hệ quản trị viên.");
                return Page();
            }
        }
    }
}