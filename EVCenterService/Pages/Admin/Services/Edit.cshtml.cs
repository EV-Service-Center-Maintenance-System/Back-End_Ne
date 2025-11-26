using EVCenterService.Data;
using EVCenterService.Models;
using EVCenterService.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EVCenterService.Pages.Admin.Services
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly IServiceCatalogService _service;
        private readonly EVServiceCenterContext _context;

        public EditModel(IServiceCatalogService service, EVServiceCenterContext context)
        {
            _service = service;
            _context = context;
        }

        [BindProperty]
        public ServiceCatalog Service { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            var serviceItem = await _service.GetServiceByIdAsync(id.Value);
            if (serviceItem == null)
                return NotFound();

            Service = serviceItem;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Nếu đang sửa gói Tổng quát (ID 4), ta bỏ qua lỗi validate giá (vì nó readonly)
            if (Service.ServiceId == 4)
            {
                ModelState.Remove("Service.BasePrice");
            }

            if (!ModelState.IsValid) return Page();

            // 1. Lấy dịch vụ cũ từ DB để giữ lại các giá trị không có trong form (như IncludeInChecklist)
            var serviceInDb = await _context.ServiceCatalogs.AsNoTracking().FirstOrDefaultAsync(s => s.ServiceId == Service.ServiceId);
            if (serviceInDb == null) return NotFound();

            // Giữ nguyên giá trị IncludeInChecklist cũ
            Service.IncludeInChecklist = serviceInDb.IncludeInChecklist;

            // Đính kèm và cập nhật
            _context.Attach(Service).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                // 2. LOGIC TÍNH LẠI GIÁ TỔNG (ĐÃ SỬA)
                // Tính lại tổng từ DB (bao gồm cả giá mới vừa lưu)
                var totalChecklistPrice = await _context.ServiceCatalogs
                    .Where(s => s.IncludeInChecklist == true)
                    .SumAsync(s => s.BasePrice ?? 0);

                // Cập nhật ID 4
                var generalInspection = await _context.ServiceCatalogs.FindAsync(4);
                if (generalInspection != null)
                {
                    generalInspection.BasePrice = totalChecklistPrice;
                    _context.ServiceCatalogs.Update(generalInspection);
                    await _context.SaveChangesAsync();
                }

                TempData["StatusMessage"] = $"✅ Dịch vụ '{Service.Name}' đã cập nhật. Giá 'Bảo dưỡng Tổng quát' hiện tại là {totalChecklistPrice:N0} đ.";
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi: {ex.Message}");
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}
