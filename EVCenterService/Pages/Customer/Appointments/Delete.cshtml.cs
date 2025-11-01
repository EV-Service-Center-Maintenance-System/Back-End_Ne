using EVCenterService.Data;
using EVCenterService.Models;
using EVCenterService.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EVCenterService.Pages.Customer.Appointments
{
    [Authorize(Roles = "Customer")]
    public class DeleteModel : PageModel
    {
        private readonly EVServiceCenterContext _context;
        // _bookingService kh�ng c�n ???c d�ng n?u ch? c?p nh?t Status, nh?ng ta c? gi? l?i
        private readonly ICustomerBookingService _bookingService;

        public DeleteModel(EVServiceCenterContext context, ICustomerBookingService bookingService)
        {
            _context = context;
            _bookingService = bookingService;
        }

        [BindProperty]
        public OrderService Booking { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            Booking = await _context.OrderServices
                .Include(o => o.Vehicle)
                .Include(o => o.OrderDetails)
                    .ThenInclude(d => d.Service)
                .FirstOrDefaultAsync(o => o.OrderId == id && o.UserId == userId);

            if (Booking == null) return NotFound();

            // S?A: Ki?m tra n?u kh�ng ph?i Pending th� kh�ng cho h?y
            if (Booking.Status != "Pending")
            {
                TempData["ErrorMessage"] = "Kh�ng th? h?y l?ch h?n ?� ???c x�c nh?n ho?c ?ang x? l�.";
                return RedirectToPage("Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var booking = await _context.OrderServices
                .FirstOrDefaultAsync(o => o.OrderId == id && o.UserId == userId);

            if (booking == null) return NotFound();

            // S?A: Ch? cho ph�p h?y n?u l� "Pending"
            if (booking.Status != "Pending")
            {
                TempData["ErrorMessage"] = "Kh�ng th? h?y l?ch h?n ?� ???c x�c nh?n ho?c ?ang x? l�.";
                return RedirectToPage("Index");
            }

            // S?A: Thay v� x�a, ch�ng ta c?p nh?t tr?ng th�i
            // _context.OrderDetails.RemoveRange(booking.OrderDetails);
            // _context.OrderServices.Remove(booking);

            booking.Status = "Cancelled";
            _context.OrderServices.Update(booking);

            await _context.SaveChangesAsync();

            TempData["StatusMessage"] = "?� h?y l?ch h?n th�nh c�ng.";
            return RedirectToPage("Index");
        }
    }
}