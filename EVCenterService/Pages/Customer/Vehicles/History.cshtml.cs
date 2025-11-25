using EVCenterService.Data;
using EVCenterService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EVCenterService.Pages.Customer.Vehicles
{
    [Authorize(Roles = "Customer")]
    public class HistoryModel : PageModel
    {
        private readonly EVServiceCenterContext _context;

        public HistoryModel(EVServiceCenterContext context)
        {
            _context = context;
        }

        public Vehicle Vehicle { get; set; } = default!;
        public List<OrderService> MaintenanceHistory { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // 1. Lấy thông tin xe
            Vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.VehicleId == id && v.UserId == userId);

            if (Vehicle == null) return NotFound();

            // 2. Lấy lịch sử bảo dưỡng (Chỉ lấy các đơn đã Hoàn thành hoặc Đã lấy xe)
            // Status: "Completed", "TechnicianCompleted", "PickedUp"
            var validStatuses = new[] { "Completed", "PickedUp", "TechnicianCompleted" };

            MaintenanceHistory = await _context.OrderServices
                .Include(o => o.OrderDetails).ThenInclude(od => od.Service)
                .Include(o => o.Technician)
                .Where(o => o.VehicleId == id && validStatuses.Contains(o.Status))
                .OrderByDescending(o => o.AppointmentDate) // Mới nhất lên đầu
                .ToListAsync();

            return Page();
        }
    }
}