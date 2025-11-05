using EVCenterService.Data;
using EVCenterService.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EVCenterService.Repository.Repositories
{
    public class AdminDashboardRepository : IAdminDashboardRepository
    {
        private readonly EVServiceCenterContext _context;

        public AdminDashboardRepository(EVServiceCenterContext context)
        {
            _context = context;
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            // SỬA LỖI: Doanh thu nên được tính trên các hóa đơn đã thanh toán.
            // Logic cũ (o.Status == "Completed") là quá hẹp.
            // Chúng ta sẽ sum tất cả các hóa đơn. 
            // Giả định rằng một hóa đơn chỉ được tạo khi thanh toán thành công.
            // Nếu bạn có trạng thái hóa đơn (ví dụ: "Paid"), thì query này sẽ tốt hơn:
            // return await _context.Invoices.Where(i => i.Status == "Paid").SumAsync(i => i.Amount ?? 0);

            // Giải pháp hiện tại: Sum tất cả các hóa đơn
            return await _context.Invoices.SumAsync(i => i.Amount ?? 0);
        }

        public async Task<int> GetTotalInvoicesAsync()
        {
            return await _context.Invoices.CountAsync();
        }

        public async Task<int> GetTotalServicesAsync()
        {
            return await _context.ServiceCatalogs.CountAsync();
        }

        public async Task<int> GetTotalStaffAsync()
        {
            return await _context.Accounts
                .CountAsync(a => a.Role == "Staff" || a.Role == "Technician");
        }

        public async Task<List<UpcomingAppointmentDto>> GetUpcomingAppointmentsAsync(int top = 5)
        {
            // SỬA LỖI 1: Dùng giờ Việt Nam (UTC+7) để tránh lỗi múi giờ
            var vietnamNow = DateTime.UtcNow.AddHours(7);

            // SỬA LỖI 2: Loại bỏ các trạng thái không còn liên quan (Đã hủy, Đã hoàn thành)
            var excludedStatuses = new[] { "TechnicianCompleted", "Cancelled" };

            return await _context.OrderServices
                .Include(o => o.Vehicle)
                .Include(o => o.User)
                .Where(o => o.AppointmentDate > vietnamNow && // Lịch hẹn trong tương lai
                            !excludedStatuses.Contains(o.Status)) // Và không bị hủy/hoàn thành
                .OrderBy(o => o.AppointmentDate)
                .Take(top)
                .Select(o => new UpcomingAppointmentDto
                {
                    CustomerName = o.User != null ? o.User.FullName : "N/A",
                    VehicleModel = o.Vehicle != null ? o.Vehicle.Model : "Unknown",
                    Status = o.Status ?? "Pending",
                    ScheduleTime = o.AppointmentDate
                })
                .ToListAsync();
        }

        public async Task<List<MonthlyRevenueDto>> GetMonthlyRevenueAsync(int months = 6)
        {
            DateTime startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-months + 1);
            DateTime endDate = DateTime.Now.Date.AddDays(1).AddTicks(-1);

            // SỬA LỖI: Logic tính doanh thu phải nhất quán với GetTotalRevenueAsync.
            // Chúng ta sẽ truy vấn thẳng vào Bảng Hóa đơn (Invoices).
            // Giả định Bảng Invoice của bạn có một trường ngày tháng (ví dụ: CreatedAt hoặc PaymentDate)
            // và một Foreign Key `OrderId` để join ngược lại `OrderService` lấy ngày hẹn.

            // === GIẢI PHÁP 1: NẾU BẢNG INVOICE CÓ NGÀY TẠO (VÍ DỤ: CreatedAt) ===
            // (Bạn sẽ cần thêm `public DateTime CreatedAt { get; set; }` vào model Invoice)
            /* return await _context.Invoices
                .Where(i => i.CreatedAt >= startDate && i.CreatedAt <= endDate)
                .GroupBy(i => new { i.CreatedAt.Year, i.CreatedAt.Month })
                .Select(g => new MonthlyRevenueDto
                {
                    Month = new DateTime(g.Key.Year, g.Key.Month, 1),
                    TotalRevenue = g.Sum(i => i.Amount ?? 0)
                })
                .OrderBy(r => r.Month)
                .ToListAsync();
            */

            // === GIẢI PHÁP 2: GIỮ LOGIC CŨ NHƯNG MỞ RỘNG TRẠNG THÁI (NHẤT QUÁN) ===
            // Dùng logic này nếu bạn muốn group theo Ngày hẹn (AppointmentDate)

            // Lấy các trạng thái mà hóa đơn đã được tạo/thanh toán
            var validStatuses = new[] { "TechnicianCompleted", "ReadyForRepair", "InProgress", "PendingPayment" };

            var ordersWithRevenue = await _context.OrderServices
                .Include(o => o.Invoices)
                .Where(o => validStatuses.Contains(o.Status) // Mở rộng trạng thái
                            && o.AppointmentDate >= startDate
                            && o.AppointmentDate <= endDate)
                .ToListAsync();

            var monthlyRevenue = ordersWithRevenue
                .GroupBy(o => new { o.AppointmentDate.Year, o.AppointmentDate.Month })
                .Select(g => new MonthlyRevenueDto
                {
                    Month = new DateTime(g.Key.Year, g.Key.Month, 1),
                    TotalRevenue = g.Sum(o => o.Invoices.Sum(i => i.Amount ?? 0)) // Giữ logic sum invoice
                })
                .OrderBy(r => r.Month)
                .ToList();

            return monthlyRevenue;
        }
    }
}