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
            var vietnamNow = DateTime.UtcNow.AddHours(7);

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
            // Lấy ngày đầu tiên của (tháng hiện tại - 5 tháng)
            DateTime startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-months + 1);
            // Lấy ngày cuối cùng của tháng hiện tại
            DateTime endDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddDays(-1);

            var rawData = await _context.Invoices
                .Where(i => i.Status == "Paid" &&
                            i.IssueDate >= startDate &&
                            i.IssueDate <= endDate)
                .GroupBy(i => new { i.IssueDate.Value.Year, i.IssueDate.Value.Month })
                .Select(g => new {
                    g.Key.Year,
                    g.Key.Month,
                    TotalRevenue = g.Sum(i => i.Amount ?? 0)
                })
                .OrderBy(r => r.Year).ThenBy(r => r.Month)
                .ToListAsync(); // <-- Chuyển sang C# (Client evaluation)

            // BƯỚC 2: Dùng C# để định dạng lại dữ liệu (việc nó giỏi)
            var monthlyRevenue = rawData.Select(r => new MonthlyRevenueDto
            {
                Month = new DateTime(r.Year, r.Month, 1),
                TotalRevenue = r.TotalRevenue
            }).ToList();

            // (Phần này để đảm bảo 6 tháng luôn hiển thị, kể cả tháng 0đ)
            var result = new List<MonthlyRevenueDto>();
            for (int i = 0; i < months; i++)
            {
                var monthToDisplay = startDate.AddMonths(i);
                var existingData = monthlyRevenue.FirstOrDefault(m => m.Month.Year == monthToDisplay.Year && m.Month.Month == monthToDisplay.Month);

                if (existingData != null)
                {
                    result.Add(existingData);
                }
                else
                {
                    result.Add(new MonthlyRevenueDto { Month = monthToDisplay, TotalRevenue = 0 });
                }
            }
            return result;
        }
    }
}