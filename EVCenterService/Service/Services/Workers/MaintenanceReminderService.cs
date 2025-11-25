using EVCenterService.Data;
using EVCenterService.Models;
using EVCenterService.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EVCenterService.Workers
{
    public class MaintenanceReminderService : BackgroundService
    {
        private readonly ILogger<MaintenanceReminderService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        // Dùng IServiceScopeFactory thay vì inject thẳng DbContext/IEmailSender
        // vì BackgroundService là 'Singleton' và cần tạo scope riêng cho mỗi lần chạy.
        public MaintenanceReminderService(ILogger<MaintenanceReminderService> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Maintenance Reminder Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await DoWorkAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred in Maintenance Reminder Service.");
                }

                // Chờ 24 giờ trước khi chạy lại
                _logger.LogInformation("Maintenance Reminder Service is sleeping for 24 hours.");
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }

        private async Task DoWorkAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Maintenance Reminder Service is running.");

            // Tạo một scope dịch vụ mới để lấy DbContext và EmailSender
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<EVServiceCenterContext>();
                var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();

                // 1. Đặt mốc thời gian (ví dụ: 6 tháng trước)
                var reminderThreshold = DateOnly.FromDateTime(DateTime.Now.AddMonths(-6));

                // 2. Tìm tất cả các xe cần nhắc nhở
                // Bao gồm cả User (để lấy email) và Notifications (để kiểm tra spam)
                var overdueVehicles = await context.Vehicles
                    .Include(v => v.User)
                        .ThenInclude(u => u.Notifications)
                    .Where(v => v.LastMaintenanceDate.HasValue &&
                                v.LastMaintenanceDate.Value < reminderThreshold &&
                                v.User.Role == "Customer" &&
                                v.User.Status == "Active")
                    .ToListAsync(stoppingToken);

                if (!overdueVehicles.Any())
                {
                    _logger.LogInformation("No overdue vehicles found.");
                    return;
                }

                _logger.LogInformation($"Found {overdueVehicles.Count} overdue vehicles. Processing...");

                foreach (var vehicle in overdueVehicles)
                {
                    // 3. KIỂM TRA SPAM: Kiểm tra xem đã gửi nhắc nhở GẦN ĐÂY chưa
                    // (ví dụ: đã gửi 1 thông báo sau lần bảo dưỡng cuối cùng)
                    bool alreadyReminded = vehicle.User.Notifications
                        .Any(n => n.Type == "MaintenanceReminder" &&
                                  n.TriggerDate > vehicle.LastMaintenanceDate.Value.ToDateTime(TimeOnly.MinValue));

                    if (alreadyReminded)
                    {
                        _logger.LogWarning($"Skipping {vehicle.User.Email}. Already reminded.");
                        continue;
                    }

                    // 4. Chuẩn bị và Gửi Email
                    string subject = "Nhắc nhở Bảo dưỡng Định kỳ cho xe của bạn";
                    string message = $@"
                        <p>Chào {vehicle.User.FullName},</p>
                        <p>Chúng tôi nhận thấy xe <strong>{vehicle.Model} (VIN: {vehicle.Vin})</strong> của bạn đã không được bảo dưỡng định kỳ trong hơn 6 tháng (Lần cuối: {vehicle.LastMaintenanceDate.Value:dd/MM/yyyy}).</p>
                        <p>Để đảm bảo xe luôn hoạt động ở trạng thái tốt nhất, vui lòng đặt lịch hẹn bảo dưỡng mới với chúng tôi.</p>
                        <p>Trân trọng,<br>Đội ngũ EV Service Center</p>";

                    bool emailSent = await emailSender.SendEmailAsync(vehicle.User.Email, subject, message);

                    Notification notification;
                    if (emailSent)
                    {
                        // 5. GHI LẠI Notification THÀNH CÔNG
                        notification = new Notification
                        {
                            ReceiverId = vehicle.UserId,
                            Content = $"Xe {vehicle.Model} của bạn đã đến hạn bảo dưỡng định kỳ.",
                            Type = "MaintenanceReminder",
                            TriggerDate = DateTime.Now
                        };
                    }
                    else
                    {
                        // 5b. GHI LẠI Notification THẤT BẠI (để không spam)
                        notification = new Notification
                        {
                            ReceiverId = vehicle.UserId,
                            Content = $"[Lỗi] Cố gắng gửi email nhắc nhở cho xe {vehicle.Model} nhưng thất bại.",
                            Type = "MaintenanceReminder", // Vẫn dùng Type này
                            TriggerDate = DateTime.Now
                        };
                    }

                    // Thêm thông báo vào DbContext (bất kể thành công hay thất bại)
                    context.Notifications.Add(notification);

                } 

                await context.SaveChangesAsync(stoppingToken);
            }
        }
    }
}