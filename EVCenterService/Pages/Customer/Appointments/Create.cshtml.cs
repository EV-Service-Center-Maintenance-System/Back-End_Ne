using EVCenterService.Data;
using EVCenterService.Models;
using EVCenterService.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EVCenterService.Pages.Customer.Appointments
{
    [Authorize(Roles = "Customer")]
    public class CreateModel : PageModel
    {
        private readonly EVServiceCenterContext _context;
        private readonly ICustomerBookingService _bookingService;

        public CreateModel(EVServiceCenterContext context, ICustomerBookingService bookingService)
        {
            _context = context;
            _bookingService = bookingService;
        }

        [BindProperty]
        public OrderService Booking { get; set; } = new OrderService();

        [BindProperty]
        public int SelectedServiceId { get; set; } 

        [BindProperty]
        public TimeSpan SelectedTime { get; set; } 

        public SelectList VehicleList { get; set; }
        public SelectList ServiceList { get; set; }

        public async Task OnGetAsync()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var vehicles = await _context.Vehicles
                .Where(v => v.UserId == userId)
                .ToListAsync();

            var services = await _context.ServiceCatalogs
                .OrderBy(s => s.Name)
                .ToListAsync();

            VehicleList = new SelectList(vehicles, "VehicleId", "Model");

            ServiceList = new SelectList(
                services.Select(s => new
                {
                    s.ServiceId,
                    Text = $"{s.Name} - {(s.BasePrice ?? 0):N0} đ"
                }),
                "ServiceId",
                "Text"
            );
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            Booking.UserId = userId;
            Booking.AppointmentDate = Booking.AppointmentDate.Date + SelectedTime;
            Booking.Status = "Chờ xác nhận";

            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            var newOrder = await _bookingService.CreateBookingAsync(Booking, SelectedServiceId);

            var service = await _context.ServiceCatalogs.FirstOrDefaultAsync(s => s.ServiceId == SelectedServiceId);
            var newDetail = new OrderDetail
            {
                OrderId = newOrder.OrderId,
                ServiceId = SelectedServiceId,
                Quantity = 1,
                UnitPrice = service?.BasePrice ?? 0
            };

            _context.OrderDetails.Add(newDetail);
            await _context.SaveChangesAsync();

            TempData["Message"] = " Đặt lịch thành công! Vui lòng chờ xác nhận.";
            return RedirectToPage("/Customer/Appointments/Index");
        }
    }
}
