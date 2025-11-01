using EVCenterService.Repository.Interfaces;
using EVCenterService.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EVCenterService.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly IAdminDashboardService _dashboardService;

        public IndexModel(IAdminDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public decimal TotalRevenue { get; set; }
        public int TotalInvoices { get; set; }
        public int TotalServices { get; set; }
        public int TotalStaff { get; set; }

        public List<UpcomingAppointmentDto> UpcomingAppointments { get; set; } = new();

        public List<MonthlyRevenueDto> MonthlyRevenue { get; set; } = new();

        public async Task OnGetAsync()
        {
            TotalRevenue = await _dashboardService.GetTotalRevenueAsync();
            TotalInvoices = await _dashboardService.GetTotalInvoicesAsync();
            TotalServices = await _dashboardService.GetTotalServicesAsync();
            TotalStaff = await _dashboardService.GetTotalStaffAsync();

            UpcomingAppointments = await _dashboardService.GetUpcomingAppointmentsAsync();
            MonthlyRevenue = await _dashboardService.GetMonthlyRevenueAsync(6);
        }
    }
}
