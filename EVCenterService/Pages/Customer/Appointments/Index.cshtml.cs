using EVCenterService.Data;
using EVCenterService.Models;
using EVCenterService.Service.Interfaces;
using EVCenterService.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EVCenterService.Pages.Customer.Appointments
{
    [Authorize(Roles = "Customer")]
    public class IndexModel : PageModel
    {
        private readonly ICustomerBookingService _service;
        private readonly EVServiceCenterContext _context;
        public IndexModel(ICustomerBookingService service, EVServiceCenterContext context)
        {
            _service = service;
            _context = context;
        }

        //public IEnumerable<OrderService> Bookings { get; set; }

        public List<AppointmentHistoryViewModel> Bookings { get; set; } = new();

        public async Task OnGetAsync()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            Bookings = await _service.GetAppointmentHistoryAsync(userId);
        }
    }
}
