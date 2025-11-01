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

        [BindProperty] public OrderService Booking { get; set; } = new();
        [BindProperty] public List<int> SelectedServiceIds { get; set; } = new();
        [BindProperty] public TimeSpan SelectedTime { get; set; }

        public SelectList VehicleList { get; set; }
        public List<ServiceCatalog> ServiceList { get; set; }

        public async Task OnGetAsync()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            VehicleList = new SelectList(
                await _context.Vehicles.Where(v => v.UserId == userId).ToListAsync(),
                "VehicleId", "Model"
            );

            ServiceList = await _context.ServiceCatalogs
                .OrderBy(s => s.Name)
                .AsNoTracking()
                .ToListAsync();

            Booking.AppointmentDate = DateTime.Now;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            Booking.UserId = userId;
            Booking.AppointmentDate = Booking.AppointmentDate.Date + SelectedTime;
            Booking.Status = "pending";

            // Validate giờ hẹn
            var workStart = new TimeSpan(7, 0, 0);
            var workEnd = new TimeSpan(19, 0, 0);
            var now = DateTime.Now;

            if (SelectedTime < workStart || SelectedTime > workEnd)
            {
                ModelState.AddModelError("SelectedTime", "Giờ hẹn phải nằm trong khung 07:00 – 19:00.");
            }
            else if (Booking.AppointmentDate.Date == now.Date && SelectedTime < now.TimeOfDay)
            {
                ModelState.AddModelError("SelectedTime", "Không thể chọn giờ đã qua trong hôm nay.");
            }

            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            // 🔹 Tính tổng giá từ tất cả dịch vụ được chọn
            var services = await _context.ServiceCatalogs
                .Where(s => SelectedServiceIds.Contains(s.ServiceId))
                .ToListAsync();

            var total = services.Sum(s => s.BasePrice ?? 0);
            Booking.TotalCost = total;

            // 🔹 Tạo OrderService
            var newOrder = await _bookingService.CreateBookingAsync(Booking, 0); // serviceId không dùng nữa

            // 🔹 Tạo nhiều OrderDetail
            foreach (var s in services)
            {
                _context.OrderDetails.Add(new OrderDetail
                {
                    OrderId = newOrder.OrderId,
                    ServiceId = s.ServiceId,
                    Quantity = 1,
                    UnitPrice = s.BasePrice ?? 0
                });
            }

            await _context.SaveChangesAsync();

            TempData["Message"] = "Order Successfull! Please waiting to approve.";
            return RedirectToPage("/Customer/Appointments/Index");
        }
    }
}
