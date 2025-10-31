using EVCenterService.Repository.Interfaces;

namespace EVCenterService.Service.Interfaces
{
    public interface IAdminDashboardService
    {
        Task<decimal> GetTotalRevenueAsync();
        Task<int> GetTotalInvoicesAsync();
        Task<int> GetTotalServicesAsync();
        Task<int> GetTotalStaffAsync();
        Task<List<UpcomingAppointmentDto>> GetUpcomingAppointmentsAsync();
        Task<List<MonthlyRevenueDto>> GetMonthlyRevenueAsync(int months = 6);
    }
}
