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
    public class CreateModel : PageModel
    {
        private readonly IServiceCatalogService _service;
        private readonly EVServiceCenterContext _context;

        public CreateModel(IServiceCatalogService service, EVServiceCenterContext context)
        {
            _service = service;
            _context = context;
        }

        [BindProperty]
        public ServiceCatalog Service { get; set; } = new();

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            // 1. Tạo dịch vụ mới (như cũ)
            // Mặc định dịch vụ mới sẽ được thêm vào checklist (nếu bạn muốn thế)
            // Hoặc bạn có thể thêm checkbox "Include in General Inspection" ở giao diện
            Service.IncludeInChecklist = true;

            await _service.CreateServiceAsync(Service);

            // 2. LOGIC MỚI: Cập nhật giá cho "Bảo dưỡng Tổng quát" (ID = 4)
            // Lưu ý: Để làm được việc này, bạn cần truy cập DbContext.
            // Nếu IServiceCatalogService không hỗ trợ, bạn nên inject DbContext vào đây.

            // Giả sử bạn đã inject EVServiceCenterContext vào CreateModel (xem bên dưới)

            // Tính tổng giá mới của tất cả dịch vụ con
            var totalChecklistPrice = await _context.ServiceCatalogs
                .Where(s => s.IncludeInChecklist == true)
                .SumAsync(s => s.BasePrice ?? 0);

            // Tìm gói "Bảo dưỡng Tổng quát"
            var generalInspection = await _context.ServiceCatalogs.FindAsync(4);
            if (generalInspection != null)
            {
                generalInspection.BasePrice = totalChecklistPrice;
                _context.ServiceCatalogs.Update(generalInspection);
                await _context.SaveChangesAsync();
            }

            TempData["StatusMessage"] = $"✅ Dịch vụ '{Service.Name}' đã được tạo. Giá gói 'Bảo dưỡng Tổng quát' đã được cập nhật thành {totalChecklistPrice:N0} đ.";
            return RedirectToPage("./Index");
        }
    }
}
