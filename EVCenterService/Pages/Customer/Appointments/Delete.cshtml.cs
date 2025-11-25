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

            if (Booking.Status != "Pending")
            {
                TempData["ErrorMessage"] = "Không thể hủy lịch hẹn đã được xác nhận hoặc đang xử lý.";
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

            if (booking.Status != "Pending")
            {
                TempData["ErrorMessage"] = "Không thể hủy lịch hẹn đã được xác nhận hoặc đang xử lý.";
                return RedirectToPage("Index");
            }


            booking.Status = "Cancelled";
            _context.OrderServices.Update(booking);

            await _context.SaveChangesAsync();

            try
            {
                if (booking.User != null)
                {
                    var serviceNames = string.Join(", ", booking.OrderDetails.Select(od => od.Service?.Name ?? "N/A"));
                    var subject = $"Xác nhận Hủy Lịch hẹn #{booking.OrderId}";
                    var message = $@"
                        <p>Chào {booking.User.FullName},</p>
                        <p>Yêu cầu hủy lịch hẹn của bạn đã được xác nhận:</p>
                        <ul>
                            <li><strong>Mã lịch hẹn:</strong> #{booking.OrderId}</li>
                            <li><strong>Ngày giờ:</strong> {booking.AppointmentDate:dd/MM/yyyy HH:mm}</li>
                            <li><strong>Dịch vụ:</strong> {serviceNames}</li>
                        </ul>
                        <p>Nếu bạn hủy nhầm, vui lòng liên hệ chúng tôi hoặc đặt lại lịch hẹn mới.</p>
                        <p>Trân trọng,<br>Đội ngũ EV Auto Center</p>";

                    await _emailSender.SendEmailAsync(booking.User.Email, subject, message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi gửi mail hủy lịch: {ex.Message}");
            }

            TempData["StatusMessage"] = "Đã hủy lịch hẹn thành công.";
            return RedirectToPage("Index");
        }
    }
}