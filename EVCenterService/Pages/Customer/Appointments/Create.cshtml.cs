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
        private readonly IConfiguration _configuration;

        public CreateModel(EVServiceCenterContext context, ICustomerBookingService bookingService, IEmailSender emailSender, IConfiguration configuration)
        {
            _context = context;
            _bookingService = bookingService;
            _emailSender = emailSender;
            _configuration = configuration;
        }

        [BindProperty] public OrderService Booking { get; set; } = new();
        [BindProperty] public List<int> SelectedServiceIds { get; set; } = new();
        [BindProperty] public TimeSpan SelectedTime { get; set; }

        public SelectList VehicleList { get; set; }
        public List<ServiceCatalog> ServiceList { get; set; }
        public bool IsEligibleForFreeInspection { get; set; }

        private async Task<int> GetUsageCountAsync(Guid userId, DateTime startDate, DateTime endDate)
        {
            return await _context.OrderDetails
                .Include(od => od.Order)
                .CountAsync(od =>
                    od.ServiceId == 4 && // General Inspection
                    od.Order.UserId == userId &&
                    // Quan trọng: Chỉ đếm những đơn nằm trong thời hạn của gói hiện tại
                    od.Order.AppointmentDate >= startDate &&
                    od.Order.AppointmentDate <= endDate &&
                    // Không đếm đơn đã hủy
                    od.Order.Status != "Cancelled" &&
                    od.Order.Status != "Rejected" &&
                    // Chỉ đếm những đơn ĐÃ ĐƯỢC miễn phí (giá = 0)
                    od.UnitPrice == 0
                );
        }

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

            // (Để đảm bảo giá gói tổng luôn bằng tổng các gói con, bất kể DB lưu gì)
            var calculatedTotal = ServiceList
                .Where(s => s.IncludeInChecklist == true)
                .Sum(s => s.BasePrice ?? 0);

            var generalService = ServiceList.FirstOrDefault(s => s.ServiceId == 4);
            if (generalService != null)
            {
                generalService.BasePrice = calculatedTotal;
            }

            // 1. Kiểm tra xem user có gói "active" không
            var activeSubscription = await _context.Subscriptions
                .Include(s => s.Plan)
                .FirstOrDefaultAsync(s => s.UserId == userId &&
                                          s.Status == "active" &&
                                          s.EndDate >= DateTime.Now); 



            Booking.AppointmentDate = DateTime.Now;

            var now = DateTime.Now;
            int defaultHour = now.Hour;
            int defaultMinute = now.Minute;

            if (defaultHour < 7)
            { // Giờ làm việc 
                defaultHour = 7;
                defaultMinute = 0;
            }
            else if (defaultHour >= 19)
            { // Giờ làm việc 
                defaultHour = 18; // Giờ cuối cùng là 18:xx
                defaultMinute = 0;
            }

            SelectedTime = new TimeSpan(defaultHour, defaultMinute, 0);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            Booking.UserId = userId;
            Booking.AppointmentDate = Booking.AppointmentDate.Date + SelectedTime;
            Booking.Status = "Pending";

            // 1. Lấy thông số giờ
            var timeZoneId = _configuration["TimeZoneId"] ?? "SE Asia Standard Time";
            var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            var vietnamNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);

            var workStart = new TimeSpan(7, 0, 0); // 7:00 AM
            var workEnd = new TimeSpan(19, 0, 0); // 19:00 (7:00 PM)
            var bufferHours = 2; // Thời gian đệm 2 tiếng

            // 2. Kiểm tra khung giờ làm việc cơ bản (7:00 -> 18:59)
            if (SelectedTime < workStart || SelectedTime >= workEnd)
            {
                ModelState.AddModelError("SelectedTime", "Giờ làm việc của trung tâm là từ 07:00 đến 19:00.");
            }

            // 3. KIỂM TRA LỖI GẦN GIỜ ĐÓNG CỬA (ƯU TIÊN LỖI NÀY TRƯỚC)
            // Giờ đặt lịch muộn nhất = 19:00 - 2 tiếng = 17:00
            var latestBookingTime = workEnd.Subtract(new TimeSpan(bufferHours, 0, 0)); // 17:00

            if (SelectedTime > latestBookingTime)
            {
                // Nếu khách chọn 18:21 (như trong ảnh), lỗi này sẽ được kích hoạt
                ModelState.AddModelError("SelectedTime", $"Quý khách vui lòng chọn giờ cách {bufferHours} tiếng trước khi trung tâm đóng cửa (trước 17:00). Vui lòng đặt lịch ngày hôm sau.");
            }
            else
            {
                // 4. CHỈ KIỂM TRA LỖI 2 TIẾNG ĐỆM (NẾU GIỜ HẸN HỢP LỆ (TRƯỚC 17:00))
                // Giờ đặt lịch sớm nhất = Giờ hiện tại + 2 tiếng
                var earliestBookingTime = vietnamNow.AddHours(bufferHours);

                // Ví dụ: Hiện tại là 14:00, khách đặt 15:30 -> Lỗi
                if (Booking.AppointmentDate < earliestBookingTime)
                {
                    ModelState.AddModelError(string.Empty, $"Bạn phải đặt lịch trước ít nhất {bufferHours} tiếng (tính cả thời gian di chuyển xe).");
                }
            }

            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            const int generalInspectionId = 4;
            var mainServiceIds = await _context.ServiceCatalogs
                .Where(s => s.IncludeInChecklist == true) // Lấy các dịch vụ con
                .Select(s => s.ServiceId)
                .ToListAsync();

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

            //  Tính tổng giá VÀ TỔNG THỜI GIAN từ tất cả dịch vụ được chọn
            var services = await _context.ServiceCatalogs
                .Where(s => SelectedServiceIds.Contains(s.ServiceId))
                .AsNoTracking()
                .ToListAsync();

            var generalServiceInPost = services.FirstOrDefault(s => s.ServiceId == 4);
            if (generalServiceInPost != null)
            {
                // Tính lại tổng từ tất cả dịch vụ con trong DB
                var realTotal = await _context.ServiceCatalogs
                                   .Where(s => s.IncludeInChecklist == true)
                                   .SumAsync(s => s.BasePrice ?? 0);
                generalServiceInPost.BasePrice = realTotal;
            }

            // ServiceId = 4 là "General Inspection" 
            var inspectionService = services.FirstOrDefault(s => s.ServiceId == 4);

            if (inspectionService != null)
            {
                // 1. Luôn lấy lại giá gốc từ DB để đảm bảo tính đúng đắn (tránh hack form)
                var originalServiceDB = await _context.ServiceCatalogs.FindAsync(4);
                decimal standardPrice = originalServiceDB?.BasePrice ?? 1000000;

                // Mặc định là tính tiền
                inspectionService.BasePrice = standardPrice;

                // 2. Kiểm tra xem user có gói "active" không
                var activeSubscription = await _context.Subscriptions
                    .Include(s => s.Plan)
                    .FirstOrDefaultAsync(s => s.UserId == userId &&
                                              s.Status == "active" &&
                                              s.EndDate >= DateTime.Now);

                if (activeSubscription != null)
                {
                    bool isPremium = activeSubscription.Plan.Code.Equals("PREMIUM", StringComparison.OrdinalIgnoreCase);
                    int limit = isPremium ? 3 : 1;

                    // 3. Đếm số lần ĐÃ DÙNG (đã đặt thành công và có giá 0đ)
                    // Lưu ý: Ngày đếm phải tính từ StartDate của gói
                    var usageCount = await GetUsageCountAsync(userId, activeSubscription.StartDate, activeSubscription.EndDate);

                    if (usageCount < limit)
                    {
                        // CÒN LƯỢT -> MIỄN PHÍ
                        inspectionService.BasePrice = 0;

                        // Ghi chú vào đơn hàng để khách biết tại sao 0đ
                        Booking.ChecklistNote = (Booking.ChecklistNote ?? "") +
                                                $"\n[SYSTEM: Áp dụng miễn phí kiểm tra lần thứ ({usageCount + 1}/{limit}) - Gói {activeSubscription.Plan.Name}]";
                    }
                    else
                    {
                        // HẾT LƯỢT -> TÍNH PHÍ BÌNH THƯỜNG
                        // Không cần làm gì thêm vì bên trên đã gán giá gốc rồi.
                        // Có thể thêm ghi chú nếu muốn rõ ràng
                        Booking.ChecklistNote = (Booking.ChecklistNote ?? "") +
                                                $"\n[SYSTEM: Đã dùng hết {limit}/{limit} lượt miễn phí. Áp dụng phí tiêu chuẩn.]";
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


            //  Tạo OrderService
            var newOrder = await _bookingService.CreateBookingAsync(Booking, 0); 

            //  Tạo nhiều OrderDetail
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
