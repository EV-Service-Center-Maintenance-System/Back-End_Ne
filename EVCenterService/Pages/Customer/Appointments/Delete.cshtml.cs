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
        private readonly ICustomerBookingService _bookingService;
        private readonly IEmailSender _emailSender;

        public DeleteModel(EVServiceCenterContext context, ICustomerBookingService bookingService, IEmailSender emailSender)
        {
            _context = context;
            _bookingService = bookingService;
            _emailSender = emailSender;
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

            // S?A: Ki?m tra n?u không ph?i Pending thì không cho h?y
            if (Booking.Status != "Pending")
            {
                TempData["ErrorMessage"] = "Không th? h?y l?ch h?n ?ã ???c xác nh?n ho?c ?ang x? lý.";
                return RedirectToPage("Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var booking = await _context.OrderServices
                .Include(o => o.User) 
                .Include(o => o.OrderDetails) 
                    .ThenInclude(od => od.Service) 
                .FirstOrDefaultAsync(o => o.OrderId == id && o.UserId == userId);

            if (booking == null) return NotFound();

            // S?A: Ch? cho phép h?y n?u là "Pending"
            if (booking.Status != "Pending")
            {
                TempData["ErrorMessage"] = "Không th? h?y l?ch h?n ?ã ???c xác nh?n ho?c ?ang x? lý.";
                return RedirectToPage("Index");
            }

            // S?A: Thay vì xóa, chúng ta c?p nh?t tr?ng thái
            // _context.OrderDetails.RemoveRange(booking.OrderDetails);
            // _context.OrderServices.Remove(booking);

            booking.Status = "Cancelled";
            _context.OrderServices.Update(booking);

            await _context.SaveChangesAsync();

            try
            {
                if (booking.User != null)
                {
                    var serviceNames = string.Join(", ", booking.OrderDetails.Select(od => od.Service?.Name ?? "N/A"));
                    var subject = $"Xác nh?n H?y L?ch h?n #{booking.OrderId}";
                    var message = $@"
                        <p>Chào {booking.User.FullName},</p>
                        <p>Yêu c?u h?y l?ch h?n c?a b?n ?ã ???c xác nh?n:</p>
                        <ul>
                            <li><strong>Mã l?ch h?n:</strong> #{booking.OrderId}</li>
                            <li><strong>Ngày gi?:</strong> {booking.AppointmentDate:dd/MM/yyyy HH:mm}</li>
                            <li><strong>D?ch v?:</strong> {serviceNames}</li>
                        </ul>
                        <p>N?u b?n h?y nh?m, vui lòng liên h? chúng tôi ho?c ??t l?i l?ch h?n m?i.</p>
                        <p>Trân tr?ng,<br>??i ng? EV Service Center</p>";

                    await _emailSender.SendEmailAsync(booking.User.Email, subject, message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"L?i g?i mail h?y l?ch: {ex.Message}");
            }

            TempData["StatusMessage"] = "?ã h?y l?ch h?n thành công.";
            return RedirectToPage("Index");
        }
    }
}