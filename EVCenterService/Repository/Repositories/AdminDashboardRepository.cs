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
            return await _context.OrderServices
                .Where(o => o.Status == "Completed")
                .Include(o => o.Invoices)
                .SelectMany(o => o.Invoices)
                .SumAsync(i => i.Amount ?? 0);
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
            var now = DateTime.Now;
            return await _context.OrderServices
                .Include(o => o.Vehicle)
                .Include(o => o.User)
                .Where(o => o.AppointmentDate > now)
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
            DateTime startDate = DateTime.Now.AddMonths(-months + 1);
            DateTime endDate = DateTime.Now;

            var completedOrders = await _context.OrderServices
                .Include(o => o.Invoices)
                .Where(o => o.Status == "Completed"
                            && o.AppointmentDate >= startDate
                            && o.AppointmentDate <= endDate)
                .ToListAsync();

            var monthlyRevenue = completedOrders
                .GroupBy(o => new { o.AppointmentDate.Year, o.AppointmentDate.Month })
                .Select(g => new MonthlyRevenueDto
                {
                    Month = new DateTime(g.Key.Year, g.Key.Month, 1),
                    TotalRevenue = g.Sum(o => o.Invoices.Sum(i => i.Amount ?? 0))
                })
                .OrderBy(r => r.Month)
                .ToList();

            return monthlyRevenue;
        }
    }
}
