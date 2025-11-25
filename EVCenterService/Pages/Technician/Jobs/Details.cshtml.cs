using EVCenterService.Data;
using EVCenterService.Models;
using EVCenterService.Service.Interfaces;
using EVCenterService.Pages.Staff.Appointments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVCenterService.Pages.Technician.Jobs
{
    [Authorize(Roles = "Technician")]
    public class DetailsModel : PageModel
    {
        private readonly ITechnicianJobService _jobService;
        private readonly EVServiceCenterContext _context;
        private readonly IEmailSender _emailSender;

        public DetailsModel(ITechnicianJobService jobService, EVServiceCenterContext context, IEmailSender emailSender)
        {
            _jobService = jobService;
            _context = context;
            _emailSender = emailSender;
        }

        public OrderService Job { get; set; } = default!;

        [BindProperty]
        public string? TechnicianNote { get; set; }

        public decimal ServiceTotalCost { get; set; }
        public List<SelectListItem> AvailableParts { get; set; } = new();

        [BindProperty]
        public List<PartUsageInput> PartsUsedInput { get; set; } = new();
        public bool IsGeneralInspection { get; set; } = false;


        public async Task<IActionResult> OnGetAsync(int id)
        {
            Job = await _jobService.GetJobDetailAsync(id);
            if (Job == null)
                return NotFound();

            // Kiểm tra xem đơn hàng này có dịch vụ "General Inspection" (ID=4) không
            bool hasGeneralInspection = Job.OrderDetails.Any(od => od.ServiceId == 4);

            // Chỉ hiển thị Checklist NẾU trạng thái là "InProgress" VÀ là "General Inspection"
            if (Job.Status == "InProgress" && hasGeneralInspection)
            {
                IsGeneralInspection = true;
            }

            if (Job.Status == "InProgress")
            {
                ServiceTotalCost = Job.OrderDetails.Sum(od => (od.UnitPrice ?? 0) * (od.Quantity ?? 1));
                await LoadAvailableParts();

                PartsUsedInput = Job.PartsUseds
                    .Where(pu => pu.PartId.HasValue) 
                    .Select(pu => new PartUsageInput
                    {
                        PartId = pu.PartId.Value, 
                        Quantity = pu.Quantity,
                        Note = pu.Note
                    }).ToList();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostSubmitQuoteAsync(int id)
        {
            Job = await _jobService.GetJobDetailAsync(id);

            if (Job == null) return NotFound();

            var slot = await _context.Slots.FirstOrDefaultAsync(s => s.OrderId == id);

            // Hàm helper để tải lại dữ liệu khi có lỗi
            var reloadDataOnError = async () =>
            {
                ServiceTotalCost = Job.OrderDetails.Sum(od => (od.UnitPrice ?? 0) * (od.Quantity ?? 1));
                await LoadAvailableParts();
            };

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Giả định rằng Staff đã phân công KTV và Slot đã được tạo 
                var centerId = Job.Slot?.CenterId;
                if (centerId == null)
                {
                    throw new InvalidOperationException("Lỗi: Không thể xác định trung tâm dịch vụ cho đơn hàng này.");
                }

                decimal totalPartsCost = 0;

                // Xóa PartsUsed cũ (nếu KTV submit lại báo giá)
                var existingPartsUsed = await _context.PartsUseds.Where(pu => pu.OrderId == id).ToListAsync();
                if (existingPartsUsed.Any())
                {
                    _context.PartsUseds.RemoveRange(existingPartsUsed);
                }

                // Lặp qua các phụ tùng KTV đã nhập
                foreach (var inputPart in PartsUsedInput.Where(p => p.PartId > 0 && p.Quantity > 0))
                {
                    var part = await _context.Parts.FindAsync(inputPart.PartId);
                    if (part == null) throw new Exception($"Không tìm thấy phụ tùng ID {inputPart.PartId}");


                    // GHI NHẬN phụ tùng đã dùng (chỉ ghi vào PartsUsed, KHÔNG trừ kho ở bước này)
                    var partsUsedEntry = new PartsUsed
                    {
                        OrderId = id,
                        PartId = inputPart.PartId,
                        Quantity = inputPart.Quantity,
                        Note = inputPart.Note
                    };
                    _context.PartsUseds.Add(partsUsedEntry);

                    // Tính toán chi phí ước tính
                    totalPartsCost += (part.UnitPrice ?? 0) * inputPart.Quantity;
                }

                // Cập nhật OrderService
                decimal serviceCost = Job.OrderDetails.Sum(od => (od.UnitPrice ?? 0) * (od.Quantity ?? 1));
                Job.TotalCost = serviceCost + totalPartsCost;
                Job.Status = "PendingQuote"; 
                if (!string.IsNullOrWhiteSpace(TechnicianNote))
                {
                    Job.ChecklistNote = (Job.ChecklistNote ?? "") + $"\nTechnician note: {TechnicianNote}";
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["Message"] = "Đã ghi nhận phụ tùng (chưa trừ kho), và gửi báo giá thành công cho Staff duyệt.";
                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError(string.Empty, $"Lỗi: {ex.Message}");
                await reloadDataOnError();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostCompleteAsync(int id)
        {
            // Khi hoàn thành: thực hiện trừ kho dựa vào PartsUseds của Order rồi mới mark complete.
            try
            {
                // Load job with parts and slot info
                var job = await _context.OrderServices
                    .Include(o => o.PartsUseds)
                    .Include(o => o.Slot)
                    .FirstOrDefaultAsync(o => o.OrderId == id);

                if (job == null) throw new Exception("Không tìm thấy Job.");

                var centerId = job.Slot?.CenterId;
                if (centerId == null) throw new Exception("Không thể xác định trung tâm dịch vụ để trừ kho.");

                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // Trừ kho cho mỗi PartsUsed
                    foreach (var partUsed in job.PartsUseds)
                    {
                        if (!partUsed.PartId.HasValue) continue;

                        var storageItem = await _context.Storages
                            .FirstOrDefaultAsync(s => s.CenterId == centerId && s.PartId == partUsed.PartId);

                        var part = await _context.Parts.FindAsync(partUsed.PartId);
                        if (storageItem == null)
                        {
                            throw new Exception($"Không tìm thấy phụ tùng '{part?.Name}' trong kho.");
                        }
                        if (storageItem.Quantity < partUsed.Quantity)
                        {
                            throw new Exception($"Không đủ tồn kho cho '{part?.Name}'. Yêu cầu: {partUsed.Quantity}, Chỉ còn: {storageItem.Quantity}.");
                        }

                        storageItem.Quantity -= partUsed.Quantity;
                        _context.Storages.Update(storageItem);

                        // Kiểm tra ngưỡng và tạo notification nếu cần
                        if (storageItem.MinThreshold.HasValue && storageItem.Quantity <= storageItem.MinThreshold)
                        {
                            var adminUser = await _context.Accounts.FirstOrDefaultAsync(a => a.Role == "Admin");
                            if (adminUser != null)
                            {
                                _context.Notifications.Add(new Notification
                                {
                                    ReceiverId = adminUser.UserId,
                                    Content = $"Cảnh báo tồn kho: Phụ tùng '{part?.Name}' (ID: {partUsed.PartId}) tại trung tâm {centerId} chỉ còn {storageItem.Quantity} (ngưỡng: {storageItem.MinThreshold}). Vui lòng refill.",
                                    Type = "StockWarning",
                                    TriggerDate = DateTime.Now
                                });
                            }
                        }
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }

                // Sau khi trừ kho thành công, đánh dấu complete bằng service
                await _jobService.CompleteJobAsync(id, TechnicianNote);
                TempData["Message"] = "Công việc đã được đánh dấu hoàn thành và kho đã được cập nhật.";

                // Gửi email thông báo hoàn thành (giữ nguyên logic hiện tại)
                var completedJob = await _context.OrderServices
                    .Include(o => o.User)
                    .Include(o => o.Vehicle) 
                    .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.Service)
                    .FirstOrDefaultAsync(o => o.OrderId == id);

                if (completedJob?.User != null)
                {
                    try
                    {
                        var userAccount = completedJob.User;
                        var serviceNames = string.Join(", ", completedJob.OrderDetails.Select(s => s.Service?.Name));
                        var vehicle = completedJob.Vehicle;

                        var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                        var vietnamNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);

                        var pickupBufferHours = 1;
                        var projectedPickupTime = vietnamNow.AddHours(pickupBufferHours);
                        var workCloseTime = new TimeSpan(19, 0, 0);
                        var workOpenTime = new TimeSpan(8, 0, 0);

                        string pickupMessage;
                        if (projectedPickupTime.TimeOfDay > workCloseTime || projectedPickupTime.Date > vietnamNow.Date)
                        {
                            var nextDay = vietnamNow.Date.AddDays(1);
                            var pickupDate = new DateTime(nextDay.Year, nextDay.Month, nextDay.Day,
                                                          workOpenTime.Hours, workOpenTime.Minutes, 0);

                            pickupMessage = $"Bạn có thể đến nhận xe vào <strong>sáng ngày hôm sau (từ {pickupDate:HH:mm} ngày {pickupDate:dd/MM/yyyy})</strong>.";
                        }
                        else
                        {
                            pickupMessage = $"Bạn có thể đến nhận xe <strong>từ {projectedPickupTime:HH:mm} ngày {projectedPickupTime:dd/MM/yyyy}</strong>.";
                        }

                        var subject = "Thông báo: Dịch vụ xe của bạn đã hoàn tất";
                        var message = $@"
                            <p>Chào {userAccount.FullName},</p>
                            <p>Chúng tôi vui mừng thông báo rằng công việc bảo dưỡng/sửa chữa cho xe của bạn đã <strong>hoàn tất</strong>.</p>
                            <ul>
                                <li><strong>Mã lịch hẹn:</strong> {completedJob.OrderId}</li>
                                <li><strong>Dịch vụ:</strong> {serviceNames}</li>
                                <li><strong>Số Km (ghi nhận khi đặt):</strong> {vehicle?.Mileage:N0} km</li>
                                <li><strong>Tổng chi phí:</strong> {completedJob.TotalCost:N0} đ</li>
                                <li><strong>Trạng thái:</strong> Đã hoàn thành</li>
                            </ul>
                            <p><strong>Dự kiến nhận xe:</strong> {pickupMessage}</p>
                            <p>Cảm ơn bạn đã sử dụng dịch vụ của EV Auto Center.</p>
                            <p>Trân trọng,</p>";

                        await _emailSender.SendEmailAsync(userAccount.Email, subject, message);
                    }
                    catch (Exception ex_email)
                    {
                        Console.WriteLine($"Lỗi gửi mail thông báo hoàn thành: {ex_email.Message}");
                    }
                }

                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                Job = await _jobService.GetJobDetailAsync(id);
                return Page();
            }
        }

        public async Task<IActionResult> OnPostCancelAsync(int id)
        {
            await _jobService.CancelJobAsync(id, TechnicianNote);
            TempData["Message"] = " Công việc đã bị hủy.";
            return RedirectToPage("Index");
        }

        private async Task LoadAvailableParts()
        {
            AvailableParts = await _context.Parts
                                   .OrderBy(p => p.Name)
                                   .Select(p => new SelectListItem
                                   {
                                       Value = p.PartId.ToString(),
                                       Text = $"{p.Name} ({p.Brand ?? "N/A"}) - {p.UnitPrice:N0} vnd"
                                   }).ToListAsync();
        }
    }
}