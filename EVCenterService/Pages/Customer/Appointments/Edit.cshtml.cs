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

    public class EditModel : PageModel
    {
        private readonly EVServiceCenterContext _context;
        private readonly ICustomerBookingService _bookingService;

        public EditModel(EVServiceCenterContext context, ICustomerBookingService bookingService)
        {
            _context = context;
            _bookingService = bookingService;
        }

        [BindProperty] public OrderService Booking { get; set; } = new();
        [BindProperty] public List<int> SelectedServiceIds { get; set; } = new();
        [BindProperty] public TimeSpan SelectedTime { get; set; }

        public SelectList VehicleList { get; set; }
        public List<ServiceCatalog> ServiceList { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            Booking = await _context.OrderServices
                .Include(o => o.Vehicle)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Service)
                .FirstOrDefaultAsync(o => o.OrderId == id && o.UserId == userId);

            if (Booking == null) return NotFound();

            // SỬA: Thêm kiểm tra trạng thái
            if (Booking.Status != "Pending")
            {
                TempData["ErrorMessage"] = "Bạn không thể sửa lịch hẹn đã được xác nhận hoặc đang xử lý.";
                return RedirectToPage("Index");
            }
            // --- Kết thúc sửa ---

            VehicleList = new SelectList(
                await _context.Vehicles.Where(v => v.UserId == userId).ToListAsync(),
                "VehicleId", "Model"
            );

            ServiceList = await _context.ServiceCatalogs
                .OrderBy(s => s.Name)
                .ToListAsync();

            SelectedServiceIds = Booking.OrderDetails.Select(d => d.ServiceId ?? 0).ToList();
            SelectedTime = Booking.AppointmentDate.TimeOfDay;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var existingOrder = await _context.OrderServices
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.OrderId == id && o.UserId == userId);

            if (existingOrder == null) return NotFound();

            // SỬA: Thêm kiểm tra trạng thái
            if (existingOrder.Status != "Pending")
            {
                TempData["ErrorMessage"] = "Bạn không thể sửa lịch hẹn đã được xác nhận hoặc đang xử lý.";
                return RedirectToPage("Index");
            }
            // --- Kết thúc sửa ---

            if (!ModelState.IsValid)
            {
                await OnGetAsync(id); // Tải lại dữ liệu cho form
                return Page();
            }

            existingOrder.VehicleId = Booking.VehicleId;
            existingOrder.ChecklistNote = Booking.ChecklistNote;
            existingOrder.AppointmentDate = Booking.AppointmentDate.Date + SelectedTime;
            existingOrder.Status = "Pending"; // Giữ nguyên trạng thái Pending

            // ===== BẮT ĐẦU LOGIC KIỂM TRA CHỒNG CHÉO LỊCH (TRANG EDIT) =====
            var selectedServicesForDuration = await _context.ServiceCatalogs
                .Where(s => SelectedServiceIds.Contains(s.ServiceId))
                .ToListAsync();

            var totalDuration = selectedServicesForDuration.Sum(s => s.DurationMinutes ?? 0);
            var newStartTime = existingOrder.AppointmentDate; // Giờ mới
            var newEndTime = newStartTime.AddMinutes(totalDuration);

            // Tìm các lịch hẹn khác (BỎ QUA CHÍNH LỊCH NÀY)
            var existingOrders = await _context.OrderServices
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Service)
                .Where(o => o.Status != "Cancelled" &&
                            o.Status != "Completed" &&
                            o.OrderId != existingOrder.OrderId) // <--- Bỏ qua chính nó
                .ToListAsync();

            bool isOverlapping = false;
            foreach (var order in existingOrders)
            {
                var existingStartTime = order.AppointmentDate;
                var existingDuration = order.OrderDetails.Sum(od => od.Service?.DurationMinutes ?? 0);
                var existingEndTime = existingStartTime.AddMinutes(existingDuration);

                if (newStartTime < existingEndTime && newEndTime > existingStartTime)
                {
                    isOverlapping = true;
                    break;
                }
            }

            if (isOverlapping)
            {
                ModelState.AddModelError(string.Empty, "Khung giờ này đã đầy hoặc không đủ thời gian cho dịch vụ bạn chọn. Vui lòng chọn giờ khác.");
                await OnGetAsync(id); // Tải lại dữ liệu cho form
                return Page();
            }
            // ===== KẾT THÚC LOGIC KIỂM TRA CHỒNG CHÉO LỊCH =====

            _context.OrderDetails.RemoveRange(existingOrder.OrderDetails);

            var selectedServices = await _context.ServiceCatalogs
                .Where(s => SelectedServiceIds.Contains(s.ServiceId))
                .ToListAsync();

            foreach (var service in selectedServices)
            {
                _context.OrderDetails.Add(new OrderDetail
                {
                    OrderId = existingOrder.OrderId,
                    ServiceId = service.ServiceId,
                    Quantity = 1,
                    UnitPrice = service.BasePrice ?? 0
                });
            }

            existingOrder.TotalCost = selectedServices.Sum(s => s.BasePrice ?? 0);

            await _context.SaveChangesAsync();

            TempData["StatusMessage"] = "Cập nhật lịch hẹn thành công!"; // Sửa lại thông báo
            return RedirectToPage("Index");
        }
    }
}
