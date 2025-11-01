using EVCenterService.Models;

namespace EVCenterService.Repository.Interfaces
{
    public interface IAdminDashboardRepository
    {
        Task<decimal> GetTotalRevenueAsync();
        Task<int> GetTotalInvoicesAsync();
        Task<int> GetTotalServicesAsync();
        Task<int> GetTotalStaffAsync();
        Task<List<UpcomingAppointmentDto>> GetUpcomingAppointmentsAsync(int top = 5);
        Task<List<MonthlyRevenueDto>> GetMonthlyRevenueAsync(int months = 6);
    }

    public class UpcomingAppointmentDto
    {
        public string CustomerName { get; set; } = "";
        public string VehicleModel { get; set; } = "";
        public string Status { get; set; } = "";
        public DateTime ScheduleTime { get; set; }
    }

    public class MonthlyRevenueDto
    {
        public DateTime Month { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
