using EVCenterService.Repository.Interfaces;
using EVCenterService.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EVCenterService.Data;
using Microsoft.EntityFrameworkCore;

namespace EVCenterService.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly IAdminDashboardService _dashboardService;
        private readonly EVServiceCenterContext _context;

        public IndexModel(IAdminDashboardService dashboardService, EVServiceCenterContext context)
        {
            _dashboardService = dashboardService;
            _context = context;
        }

        public decimal TotalRevenue { get; set; }
        public int TotalInvoices { get; set; }
        public int TotalServices { get; set; }
        public int TotalStaff { get; set; }

        public List<UpcomingAppointmentDto> UpcomingAppointments { get; set; } = new();

        public List<MonthlyRevenueDto> MonthlyRevenue { get; set; } = new();

        public double AverageRating { get; set; }
        public List<EVCenterService.Models.Feedback> RecentFeedbacks { get; set; } = new();

        public async Task OnGetAsync()
        {
            TotalRevenue = await _dashboardService.GetTotalRevenueAsync();
            TotalInvoices = await _dashboardService.GetTotalInvoicesAsync();
            TotalServices = await _dashboardService.GetTotalServicesAsync();
            TotalStaff = await _dashboardService.GetTotalStaffAsync();

            UpcomingAppointments = await _dashboardService.GetUpcomingAppointmentsAsync();
            MonthlyRevenue = await _dashboardService.GetMonthlyRevenueAsync(6);

            RecentFeedbacks = await _context.Feedbacks
                .Include(f => f.User) // Lấy tên khách hàng
                .OrderByDescending(f => f.CreatedAt)
                .Take(4) // Lấy 4 đánh giá mới nhất
                .ToListAsync();

            // 2. Lấy Điểm trung bình
            var ratings = await _context.Feedbacks.Select(f => f.Rating).ToListAsync();
            AverageRating = ratings.Average() ?? 0.0;
        }
    }
}
