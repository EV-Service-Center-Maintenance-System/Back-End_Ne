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
            //// Ki?m tra xem PartId ?� t?n t?i ch?a (v� PartId KH�NG t? t?ng trong DB c?a b?n)
            //if (await _context.Parts.AnyAsync(p => p.PartId == Part.PartId))
            //{
            //    ModelState.AddModelError("Part.PartId", "M� Ph? t�ng (ID) n�y ?� t?n t?i.");
            //}

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // B?t ??u transaction
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Th�m Part m?i
                _context.Parts.Add(Part);
                // L?u Part tr??c ?? ??m b?o PartId h?p l? cho b??c sau
                // (Quan tr?ng v� PartId kh�ng t? t?ng, b?n ph?i cung c?p n� t? form)
                await _context.SaveChangesAsync();

                // 2. L?y danh s�ch t?t c? c�c CenterID
                var centerIds = await _context.MaintenanceCenters.Select(c => c.CenterId).ToListAsync();

                // 3. T?o c�c b?n ghi Storage t??ng ?ng (KH�NG c?n t�nh to�n StorageID)
                foreach (var centerId in centerIds)
                {
                    var storageEntry = new Storage
                    {
                        // StorageId = nextStorageId++, // <-- X�A D�NG N�Y V� C�C D�NG T�NH TO�N ID ? TR�N
                        CenterId = centerId,
                        PartId = Part.PartId, // PartId l?y t? ??i t??ng Part v?a ???c th�m
                        Quantity = 0, // S? l??ng ban ??u
                        MinThreshold = 5 // Ng??ng m?c ??nh
                    };
                    _context.Storages.Add(storageEntry);
                }
                // L?u c�c b?n ghi Storage (database s? t? t?o StorageID)
                await _context.SaveChangesAsync();

                // N?u m?i th? th�nh c�ng, commit transaction
                await transaction.CommitAsync();

                TempData["StatusMessage"] = $"Ph? t�ng '{Part.Name}' ?� ???c t?o v� th�m v�o kho c�c trung t�m.";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                // N?u c� l?i, rollback transaction
                await transaction.RollbackAsync();
                Console.WriteLine($"Error creating part: {ex.Message}");
                ModelState.AddModelError(string.Empty, "?� x?y ra l?i khi t?o ph? t�ng. Vui l�ng th? l?i ho?c li�n h? qu?n tr? vi�n.");
                return Page();
            }
        }
    }
}