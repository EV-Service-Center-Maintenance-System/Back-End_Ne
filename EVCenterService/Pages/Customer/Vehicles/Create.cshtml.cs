using EVCenterService.Data;
using EVCenterService.Models;
using EVCenterService.Service.Interfaces;
using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace EVCenterService.Pages.Customer.Vehicles
{
    [Authorize(Roles = "Customer")]
    public class CreateModel : PageModel
    {
        private readonly IVehicleService _vehicleService;
        private readonly IEmailSender _emailSender;
        private readonly EVServiceCenterContext _context;

        [BindProperty]
        public Vehicle Vehicle { get; set; }

        public CreateModel(IVehicleService vehicleService, IEmailSender emailSender, EVServiceCenterContext context)
        {
            _vehicleService = vehicleService;
            _emailSender = emailSender;
            _context = context;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            if (await _vehicleService.IsVinDuplicateAsync(Vehicle.Vin))
            {
                ModelState.AddModelError("Vehicle.Vin", $"Số VIN '{Vehicle.Vin}' đã tồn tại trong hệ thống.");
                return Page();
            }

            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            Vehicle.UserId = userId;

            await _vehicleService.AddVehicleAsync(Vehicle);

            // Nếu ngày bảo dưỡng cuối cùng đã quá 6 tháng
            var sixMonthsAgo = DateOnly.FromDateTime(DateTime.Now.AddMonths(-6));

            if (Vehicle.LastMaintenanceDate.HasValue && Vehicle.LastMaintenanceDate < sixMonthsAgo)
            {
                // Lấy thông tin User để gửi mail
                var user = await _context.Accounts.FindAsync(userId);
                if (user != null)
                {
                    string subject = "Cảnh báo: Xe của bạn đã quá hạn bảo dưỡng!";
                    string message = $@"
                        <p>Chào {user.FullName},</p>
                        <p>Bạn vừa thêm xe <strong>{Vehicle.Model}</strong> vào hệ thống.</p>
                        <p style='color:red;'><strong>Lưu ý:</strong> Dựa trên dữ liệu bạn nhập, lần bảo dưỡng cuối cùng là {Vehicle.LastMaintenanceDate:dd/MM/yyyy}, đã quá hạn 6 tháng.</p>
                        <p>Chúng tôi khuyến nghị bạn đặt lịch bảo dưỡng ngay để đảm bảo an toàn.</p>
                        <p><a href='https://localhost:7177/Customer/Appointments/Create'>Đặt lịch ngay tại đây</a></p>
                        <p>Trân trọng,<br>EV Service Center</p>";

                    // Gửi email ngay lập tức
                    await _emailSender.SendEmailAsync(user.Email, subject, message);

                    // (Tùy chọn) Tạo Notification trong DB luôn nếu muốn hiển thị chuông thông báo
                    var noti = new Notification
                    {
                        ReceiverId = userId,
                        Content = $"Cảnh báo: Xe {Vehicle.Model} vừa thêm đã quá hạn bảo dưỡng.",
                        Type = "MaintenanceReminder",
                        TriggerDate = DateTime.Now,
                        IsRead = false
                    };
                    _context.Notifications.Add(noti);
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToPage("Index");
        }
    }
}
