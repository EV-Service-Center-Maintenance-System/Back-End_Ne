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

            if (!ModelState.IsValid)
            {
                return Page();
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Parts.Add(Part);

                await _context.SaveChangesAsync();

                var centerIds = await _context.MaintenanceCenters.Select(c => c.CenterId).ToListAsync();

                foreach (var centerId in centerIds)
                {
                    var storageEntry = new Storage
                    {
                        CenterId = centerId,
                        PartId = Part.PartId, 
                        Quantity = 0, 
                        MinThreshold = 5 
                    };
                    _context.Storages.Add(storageEntry);
                }
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                TempData["StatusMessage"] = $"Phụ tùng '{Part.Name}' Đã được tạo và thêm vào kho các trung tâm.";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error creating part: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi tạo phụ tùng. Vui lòng thử lại hoặc liên hệ quản trị viên.");
                return Page();
            }
        }
    }
}