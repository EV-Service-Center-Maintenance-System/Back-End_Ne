using EVCenterService.Data;
using EVCenterService.Models;
using EVCenterService.Service.Interfaces;
using EVCenterService.Pages.Staff.Appointments;
using EVCenterService.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

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

        [BindProperty]
        public decimal CurrentMileage { get; set; }

        public decimal ServiceTotalCost { get; set; }
        public List<SelectListItem> AvailableParts { get; set; } = new();

        [BindProperty]
        public List<PartUsageInput> PartsUsedInput { get; set; } = new();
        public bool IsGeneralInspection { get; set; } = false;

        public List<ServiceCatalog> ChecklistServices { get; set; } = new();

        [BindProperty]
        public bool HasRepairItem { get; set; }


        public async Task<IActionResult> OnGetAsync(int id)
        {
            Job = await _jobService.GetJobDetailAsync(id);
            if (Job == null)
                return NotFound();

            if (Job.Vehicle != null)
            {
                CurrentMileage = Job.Vehicle.Mileage ?? 0;
            }

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

            if (IsGeneralInspection)
            {
                ChecklistServices = await _context.ServiceCatalogs
                    .Where(s => s.IncludeInChecklist == true)
                    .ToListAsync();
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
                // Xác định lại IsGeneralInspection để hiển thị checklist
                bool hasGeneralInspection = Job.OrderDetails.Any(od => od.ServiceId == 4);
                if (Job.Status == "InProgress" && hasGeneralInspection) IsGeneralInspection = true;
                if (IsGeneralInspection)
                {
                    ChecklistServices = await _context.ServiceCatalogs.Where(s => s.IncludeInChecklist == true).ToListAsync();
                }
                await LoadAvailableParts();
            };

            // 1. Kiểm tra xem có phải là Bảo dưỡng tổng quát không
            bool isGeneral = Job.OrderDetails.Any(od => od.ServiceId == 4); // ID 4 = Tổng quát

            if (isGeneral)
            {
                // 2. Nếu có mục cần sửa (HasRepairItem = true)
                if (HasRepairItem)
                {
                    // Bắt buộc phải có phụ tùng được chọn
                    bool hasParts = PartsUsedInput.Any(p => p.PartId > 0 && p.Quantity > 0);

                    if (!hasParts)
                    {
                        ModelState.AddModelError(string.Empty, "⚠️ Bạn đã đánh dấu có mục CẦN SỬA (❌). Vui lòng chọn ít nhất một phụ tùng thay thế.");
                        await reloadDataOnError();
                        return Page();
                    }
                }
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Đây là bước Tech cập nhật lại dữ liệu sai của khách (nếu có)
                if (Job.Vehicle != null)
                {
                    // 1. Cập nhật vào Xe (như cũ)
                    Job.Vehicle.Mileage = CurrentMileage;
                    _context.Vehicles.Update(Job.Vehicle);

                    // 2. LƯU VÀO ĐƠN HÀNG (MỚI) -> Để giữ lịch sử
                    Job.MileageAtService = CurrentMileage; 
                    _context.OrderServices.Update(Job);
                }
                // Giả định rằng Staff đã phân công KTV và Slot đã được tạo 
                var centerId = Job.Slot?.CenterId;
                if (centerId == null)
                {
                    throw new InvalidOperationException("Lỗi: Không thể xác định trung tâm dịch vụ cho đơn hàng này. Không thể trừ kho.");
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


                    // 1. Kiểm tra kho
                    var storageItem = await _context.Storages
                                            .FirstOrDefaultAsync(s => s.CenterId == centerId && s.PartId == inputPart.PartId);

                    if (storageItem == null)
                    {
                        throw new Exception($"Phụ tùng '{part.Name}' không tồn tại trong kho của trung tâm này.");
                    }

                    if (storageItem.Quantity < inputPart.Quantity)
                    {
                        // 2. Báo lỗi không đủ hàng
                        throw new Exception($"Không đủ tồn kho cho '{part.Name}'. Yêu cầu: {inputPart.Quantity}, Chỉ còn: {storageItem.Quantity}. Vui lòng báo cáo Admin.");
                    }


                    // Ghi nhận phụ tùng đã dùng (PartsUsed)
                    var partsUsedEntry = new PartsUsed
                    {
                        OrderId = id,
                        PartId = inputPart.PartId,
                        Quantity = inputPart.Quantity,
                        Note = inputPart.Note
                    };
                    _context.PartsUseds.Add(partsUsedEntry);

                    // Tính toán chi phí
                    totalPartsCost += (part.UnitPrice ?? 0) * inputPart.Quantity;

                    // 4. Kiểm tra ngưỡng và cảnh báo Admin
                    if (storageItem.MinThreshold.HasValue && storageItem.Quantity <= storageItem.MinThreshold)
                    {
                        var adminUser = await _context.Accounts.FirstOrDefaultAsync(a => a.Role == "Admin");
                        if (adminUser != null)
                        {
                            _context.Notifications.Add(new Notification
                            {
                                ReceiverId = adminUser.UserId,
                                Content = $"Cảnh báo tồn kho: Phụ tùng '{part?.Name}' (ID: {inputPart.PartId}) tại trung tâm {centerId} chỉ còn {storageItem.Quantity} (ngưỡng: {storageItem.MinThreshold}). Vui lòng refill.",
                                Type = "StockWarning",
                                TriggerDate = DateTime.Now
                            });
                        }
                    }
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

                TempData["Message"] = "Đã cập nhật ODO, ghi nhận phụ tùng và gửi báo giá thành công cho Staff duyệt.";
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
            try
            {
                // 1. Lấy thông tin đầy đủ của đơn hàng
                var completedJob = await _context.OrderServices
                    .Include(o => o.User)
                    .Include(o => o.Vehicle)
                    .Include(o => o.OrderDetails).ThenInclude(od => od.Service)
                    .Include(o => o.PartsUseds).ThenInclude(pu => pu.Part) // Cần PartsUseds để trừ kho
                    .FirstOrDefaultAsync(o => o.OrderId == id);

                if (completedJob == null) return RedirectToPage("Index");

                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // 2. CẬP NHẬT NGÀY BẢO DƯỠNG CHO XE
                    if (completedJob.Vehicle != null)
                    {
                        // Cập nhật ngày hôm nay là ngày bảo dưỡng cuối cùng
                        completedJob.Vehicle.LastMaintenanceDate = DateOnly.FromDateTime(DateTime.Now);

                        // Lưu ý: KHÔNG cập nhật Mileage ở đây (đã làm ở bước Báo giá).
                        _context.Vehicles.Update(completedJob.Vehicle);
                    }

                    // 3. TRỪ KHO 
                    // Lấy CenterId từ Slot làm việc
                    var slot = await _context.Slots.FirstOrDefaultAsync(s => s.OrderId == id);
                    var centerId = slot?.CenterId;

                    if (centerId != null)
                    {
                        foreach (var partUsed in completedJob.PartsUseds)
                        {
                            var storageItem = await _context.Storages
                                .FirstOrDefaultAsync(s => s.CenterId == centerId && s.PartId == partUsed.PartId);

                            if (storageItem != null)
                            {
                                // Trừ số lượng thực tế
                                storageItem.Quantity -= partUsed.Quantity;

                                // Đảm bảo không bị âm (dù logic báo giá đã check, nhưng an toàn vẫn hơn)
                                if (storageItem.Quantity < 0) storageItem.Quantity = 0;

                                _context.Storages.Update(storageItem);
                            }
                        }
                    }

                    // 4. Đổi trạng thái công việc thành "TechnicianCompleted"
                    await _jobService.CompleteJobAsync(id, TechnicianNote);

                    // 5. Lưu tất cả thay đổi vào CSDL
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    // 6. GỬI EMAIL THÔNG BÁO (Với giờ nhận xe dự kiến)
                    try
                    {
                        if (completedJob.User != null)
                        {
                            var userAccount = completedJob.User;
                            var serviceNames = string.Join(", ", completedJob.OrderDetails.Select(s => s.Service?.Name));
                            var vehicle = completedJob.Vehicle;

                            // --- Tính toán giờ nhận xe ---
                            var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                            var vietnamNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);

                            var pickupBufferHours = 1; // 1 tiếng để Staff chuẩn bị giấy tờ/rửa xe
                            var projectedPickupTime = vietnamNow.AddHours(pickupBufferHours);
                            var workCloseTime = new TimeSpan(19, 0, 0); // 19:00 đóng cửa
                            var workOpenTime = new TimeSpan(8, 0, 0);   // 08:00 mở cửa hôm sau

                            string pickupMessage;

                            // Nếu giờ dự kiến vượt quá 19:00 HOẶC đã sang ngày hôm sau
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
                                <p>Kỹ thuật viên đã hoàn tất công việc bảo dưỡng/sửa chữa cho xe <strong>{vehicle?.Model}</strong>.</p>
                                <ul>
                                    <li><strong>Mã lịch hẹn:</strong> {completedJob.OrderId}</li>
                                    <li><strong>Dịch vụ thực hiện:</strong> {serviceNames}</li>
                                    <li><strong>Số Km ghi nhận:</strong> {vehicle?.Mileage:N0} km</li>
                                    <li><strong>Tổng chi phí:</strong> {completedJob.TotalCost:N0} đ</li>
                                    <li><strong>Trạng thái:</strong> Đã hoàn thành kỹ thuật</li>
                                </ul>
                                <p><strong>Dự kiến nhận xe:</strong> {pickupMessage}</p>
                                <p>Vui lòng đến quầy dịch vụ để hoàn tất thủ tục nhận xe.</p>
                                <p>Trân trọng,<br>EV Auto Center</p>";

                            await _emailSender.SendEmailAsync(userAccount.Email, subject, message);
                        }
                    }
                    catch (Exception ex_email)
                    {
                        Console.WriteLine($"Lỗi gửi mail thông báo hoàn thành: {ex_email.Message}");
                        // Không throw lỗi ở đây để đảm bảo transaction đã commit thành công
                    }

                    TempData["Message"] = "Công việc hoàn thành. Đã cập nhật hồ sơ xe, trừ kho và thông báo cho khách.";
                    return RedirectToPage("Index");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    ModelState.AddModelError(string.Empty, $"Lỗi khi lưu dữ liệu: {ex.Message}");
                    Job = await _jobService.GetJobDetailAsync(id);
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
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