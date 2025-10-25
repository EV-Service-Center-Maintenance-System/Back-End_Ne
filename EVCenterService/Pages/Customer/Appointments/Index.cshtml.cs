using EVCenterService.Data;
using EVCenterService.Models;
using EVCenterService.Service.Interfaces;
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

        public IEnumerable<OrderService> Bookings { get; set; }

        public async Task OnGetAsync()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            Bookings = await _context.OrderServices
                .Include(o => o.Vehicle)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Service).Include(o => o.Technician)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.AppointmentDate)
                .ToListAsync();
        }
    }
}
