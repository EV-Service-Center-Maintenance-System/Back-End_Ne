using EVCenterService.Repository.Interfaces;
using EVCenterService.Service.Interfaces;

namespace EVCenterService.Service.Services
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly IAdminDashboardRepository _repo;

        public AdminDashboardService(IAdminDashboardRepository repo)
        {
            _repo = repo;
        }

        public Task<decimal> GetTotalRevenueAsync() => _repo.GetTotalRevenueAsync();

        public Task<int> GetTotalInvoicesAsync() => _repo.GetTotalInvoicesAsync();

        public Task<int> GetTotalServicesAsync() => _repo.GetTotalServicesAsync();

        public Task<int> GetTotalStaffAsync() => _repo.GetTotalStaffAsync();

        public Task<List<UpcomingAppointmentDto>> GetUpcomingAppointmentsAsync()
            => _repo.GetUpcomingAppointmentsAsync();

        public Task<List<MonthlyRevenueDto>> GetMonthlyRevenueAsync(int months = 6)
            => _repo.GetMonthlyRevenueAsync(months);
    }
}
