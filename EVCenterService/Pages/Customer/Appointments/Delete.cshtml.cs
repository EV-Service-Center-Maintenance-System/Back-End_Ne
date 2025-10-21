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
    public class DeleteModel : PageModel
    {
        private readonly EVServiceCenterContext _context;
        private readonly ICustomerBookingService _bookingService;

        public DeleteModel(EVServiceCenterContext context, ICustomerBookingService bookingService)
        {
            _context = context;
            _bookingService = bookingService;
        }

        [BindProperty]
        public OrderService Booking { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            Booking = await _context.OrderServices
                .Include(o => o.Vehicle)
                .Include(o => o.OrderDetails)
                    .ThenInclude(d => d.Service)
                .FirstOrDefaultAsync(o => o.OrderId == id && o.UserId == userId);

            if (Booking == null)
                return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var booking = await _context.OrderServices
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.OrderId == id && o.UserId == userId);

            if (booking == null)
                return NotFound();

            _context.OrderDetails.RemoveRange(booking.OrderDetails);

            _context.OrderServices.Remove(booking);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Delete Successfull.";
            return RedirectToPage("Index");
        }
    }
}
