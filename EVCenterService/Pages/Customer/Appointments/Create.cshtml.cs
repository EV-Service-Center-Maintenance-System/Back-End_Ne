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
            Booking.Status = "Pending";

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

            // 🔹 Tính tổng giá VÀ TỔNG THỜI GIAN từ tất cả dịch vụ được chọn
            var services = await _context.ServiceCatalogs
                .Where(s => SelectedServiceIds.Contains(s.ServiceId))
                .ToListAsync();

            if (!services.Any())
            {
                ModelState.AddModelError("SelectedServiceIds", "Bạn phải chọn ít nhất một dịch vụ.");
                await OnGetAsync();
                return Page();
            }

            var total = services.Sum(s => s.BasePrice ?? 0);
            var totalDuration = services.Sum(s => s.DurationMinutes ?? 0); // Lấy tổng thời gian
            Booking.TotalCost = total;

            // ===== BẮT ĐẦU LOGIC KIỂM TRA CHỒNG CHÉO LỊCH =====
            var newStartTime = Booking.AppointmentDate; // Đã bao gồm giờ
            var newEndTime = newStartTime.AddMinutes(totalDuration);

            // Tìm các lịch hẹn khác (không bị hủy/hoàn thành)
            var existingOrders = await _context.OrderServices
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Service)
                .Where(o => o.Status != "Cancelled" && o.Status != "Completed")
                .ToListAsync();

            bool isOverlapping = false;
            foreach (var existingOrder in existingOrders)
            {
                var existingStartTime = existingOrder.AppointmentDate;
                var existingDuration = existingOrder.OrderDetails.Sum(od => od.Service?.DurationMinutes ?? 0);
                var existingEndTime = existingStartTime.AddMinutes(existingDuration);

                // Đây là logic kiểm tra chồng chéo:
                // (Bắt đầu MỚI < Kết thúc CŨ) VÀ (Kết thúc MỚI > Bắt đầu CŨ)
                if (newStartTime < existingEndTime && newEndTime > existingStartTime)
                {
                    isOverlapping = true;
                    break;
                }
            }

            if (isOverlapping)
            {
                ModelState.AddModelError(string.Empty, "Khung giờ này đã đầy hoặc không đủ thời gian cho dịch vụ bạn chọn. Vui lòng chọn giờ khác.");
                await OnGetAsync(); // Tải lại danh sách
                return Page();
            }
            // ===== KẾT THÚC LOGIC KIỂM TRA CHỒNG CHÉO LỊCH =====

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
