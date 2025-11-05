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
        private readonly IEmailSender _emailSender;

        public CreateModel(EVServiceCenterContext context, ICustomerBookingService bookingService, IEmailSender emailSender)
        {
            _context = context;
            _bookingService = bookingService;
            _emailSender = emailSender;
        }

        [BindProperty] public OrderService Booking { get; set; } = new();
        [BindProperty] public List<int> SelectedServiceIds { get; set; } = new();
        [BindProperty] public TimeSpan SelectedTime { get; set; }

        public SelectList VehicleList { get; set; }
        public List<ServiceCatalog> ServiceList { get; set; }
        public bool IsEligibleForFreeInspection { get; set; }

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

            // 1. Kiểm tra xem user có gói "active" không
            var activeSubscription = await _context.Subscriptions
                .FirstOrDefaultAsync(s => s.UserId == userId &&
                                          s.Status == "active" &&
                                          s.EndDate >= DateTime.Now); //

            IsEligibleForFreeInspection = false;
            if (activeSubscription != null)
            {
                // 2. Có gói. Kiểm tra xem đã dùng lần miễn phí tháng này chưa.
                var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

                var alreadyUsedFreebie = await _context.OrderDetails
                    .AnyAsync(od => od.ServiceId == 4 && // ServiceId = 4 là "General Inspection"
                                    od.Order.UserId == userId &&
                                    od.Order.AppointmentDate >= startOfMonth &&
                                    od.UnitPrice == 0); // Đã được miễn phí

                if (!alreadyUsedFreebie)
                {
                    // 3. ĐỦ ĐIỀU KIỆN MIỄN PHÍ
                    IsEligibleForFreeInspection = true;
                }
            }

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

            // ===== BẮT ĐẦU LOGIC NGHIỆP VỤ DỊCH VỤ =====
            const int generalInspectionId = 4;
            var mainServiceIds = new List<int> { 1, 2, 3 }; // Battery, Brake, Cooling

            bool hasGeneral = SelectedServiceIds.Contains(generalInspectionId);
            int mainServiceCount = SelectedServiceIds.Count(id => mainServiceIds.Contains(id));

            // KỊCH BẢN 1: Chọn cả 3 mục chính
            if (mainServiceCount == 3)
            {
                ModelState.AddModelError(string.Empty, "Bạn đã chọn cả 3 dịch vụ chính. Vui lòng chỉ chọn 'Bảo dưỡng Tổng quát' (General Inspection) vì đã bao gồm các mục này.");
                await OnGetAsync();
                return Page();
            }

            // KỊCH BẢN 2: Chọn "General" VÀ một dịch vụ chính khác
            if (hasGeneral && mainServiceCount > 0)
            {
                ModelState.AddModelError(string.Empty, "'Bảo dưỡng Tổng quát' đã bao gồm các dịch vụ khác. Vui lòng chỉ chọn 'Bảo dưỡng Tổng quát' hoặc các dịch vụ riêng lẻ.");
                await OnGetAsync();
                return Page();
            }
            // ===== KẾT THÚC LOGIC NGHIỆP VỤ DỊCH VỤ =====

            // 🔹 Tính tổng giá VÀ TỔNG THỜI GIAN từ tất cả dịch vụ được chọn
            var services = await _context.ServiceCatalogs
                .Where(s => SelectedServiceIds.Contains(s.ServiceId))
                .ToListAsync();

            // ServiceId = 4 là "General Inspection" trong CSDL của bạn
            var inspectionService = services.FirstOrDefault(s => s.ServiceId == 4);

            if (inspectionService != null) // Kiểm tra xem khách có chọn dịch vụ này không
            {
                // 1. Kiểm tra xem user có gói "active" không
                var activeSubscription = await _context.Subscriptions
                    .FirstOrDefaultAsync(s => s.UserId == userId &&
                                              s.Status == "active" &&
                                              s.EndDate >= DateTime.Now); //

                if (activeSubscription != null)
                {
                    // 2. Có gói. Kiểm tra xem đã dùng lần miễn phí tháng này chưa.
                    var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

                    var alreadyUsedFreebie = await _context.OrderDetails
                        .AnyAsync(od => od.ServiceId == 4 && // Là dịch vụ General Inspection
                                        od.Order.UserId == userId && // Của user này
                                        od.Order.AppointmentDate >= startOfMonth && // Trong tháng này
                                        od.UnitPrice == 0); // Đã được miễn phí (giá 0)

                    if (!alreadyUsedFreebie)
                    {
                        // 3. CHƯA DÙNG -> Áp dụng miễn phí
                        inspectionService.BasePrice = 0;

                        // Thêm ghi chú để Staff biết
                        Booking.ChecklistNote = (Booking.ChecklistNote ?? "") +
                                                "\n[Áp dụng miễn phí kiểm tra (Gói dịch vụ)]";
                    }
                }
            }

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

            try
            {
                // Lấy thông tin user (vì chúng ta chỉ có userId)
                var userAccount = await _context.Accounts.FindAsync(userId);
                if (userAccount != null)
                {
                    // Lấy tên các dịch vụ
                    var serviceNames = string.Join(", ", services.Select(s => s.Name));
                    var subject = "Xác nhận Đặt Lịch hẹn Mới (Chờ duyệt)";
                    var message = $@"
                        <p>Chào {userAccount.FullName},</p>
                        <p>Chúng tôi đã nhận được yêu cầu đặt lịch hẹn của bạn:</p>
                        <ul>
                            <li><strong>Ngày giờ:</strong> {Booking.AppointmentDate:dd/MM/yyyy HH:mm}</li>
                            <li><strong>Dịch vụ:</strong> {serviceNames}</li>
                            <li><strong>Tổng chi phí (tạm tính):</strong> {Booking.TotalCost:N0} đ</li>
                        </ul>
                        <p>Lịch hẹn của bạn đang ở trạng thái <strong>Chờ duyệt</strong>. Chúng tôi sẽ liên hệ lại sớm.</p>
                        <p>Trân trọng,<br>Đội ngũ EV Service Center</p>";

                    await _emailSender.SendEmailAsync(userAccount.Email, subject, message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi gửi mail đặt lịch: {ex.Message}");
                // Không dừng lại nếu gửi mail lỗi, chỉ log
            }

            TempData["Message"] = "Order Successfull! Please waiting to approve.";
            return RedirectToPage("/Customer/Appointments/Index");
        }
    }
}
