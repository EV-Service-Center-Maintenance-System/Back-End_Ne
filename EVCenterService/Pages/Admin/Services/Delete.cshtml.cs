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
    public class DeleteModel : PageModel
    {
        private readonly IServiceCatalogService _service;
        private readonly EVServiceCenterContext _context;

        public DeleteModel(IServiceCatalogService service, EVServiceCenterContext context)
        {
            _service = service;
            _context = context;
        }

        [BindProperty]
        public ServiceCatalog Service { get; set; } = default!;

        public string ErrorMessage { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();
            // Chặn xóa gói Tổng quát
            if (id == 4)
            {
                TempData["StatusMessage"] = "Không thể xóa gói dịch vụ gốc.";
                return RedirectToPage("./Index");
            }

            var serviceItem = await _service.GetServiceByIdAsync(id.Value);
            if (serviceItem == null) return NotFound();

            Service = serviceItem;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null) return NotFound();
            if (id == 4) return RedirectToPage("./Index");

            try
            {
                // 1. Xóa dịch vụ
                await _service.DeleteServiceAsync(id.Value);

                // 2. LOGIC TỰ ĐỘNG: Tính lại giá gói Tổng quát sau khi xóa
                var totalChecklistPrice = await _context.ServiceCatalogs
                    .Where(s => s.IncludeInChecklist == true)
                    .SumAsync(s => s.BasePrice ?? 0);

                var generalInspection = await _context.ServiceCatalogs.FindAsync(4);
                if (generalInspection != null)
                {
                    generalInspection.BasePrice = totalChecklistPrice;
                    _context.ServiceCatalogs.Update(generalInspection);
                    await _context.SaveChangesAsync();
                }

                TempData["StatusMessage"] = $"🗑️ Đã xóa dịch vụ. Giá 'Bảo dưỡng Tổng quát' giảm còn {totalChecklistPrice:N0} đ.";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                // Xử lý lỗi (ví dụ: khóa ngoại)
                ErrorMessage = ex.Message;
                // Load lại data để hiện trang lỗi
                var s = await _service.GetServiceByIdAsync(id.Value);
                if (s != null) Service = s;
                return Page();
            }
        }
    }
}