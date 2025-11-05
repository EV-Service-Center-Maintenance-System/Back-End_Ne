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

            // DELIVERABLE: Bắt đầu Transaction
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // SỬA LỖI: Lấy CenterID từ Slot
                // Giả định rằng Staff đã phân công KTV và Slot đã được tạo (Task 1.8)
                var centerId = Job.Slot?.CenterId;
                if (centerId == null)
                {
                    // Lỗi này xảy ra do Staff chưa gán Slot đúng cách
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

                    // === BẮT ĐẦU LOGIC TASK 2.7 ===

                    // 1. Kiểm tra kho
                    var storageItem = await _context.Storages
                                            .FirstOrDefaultAsync(s => s.CenterId == centerId && s.PartId == inputPart.PartId);

                    if (storageItem == null)
                    {
                        throw new Exception($"Phụ tùng '{part.Name}' không tồn tại trong kho của trung tâm này.");
                    }

                    if (storageItem.Quantity < inputPart.Quantity)
                    {
                        // 2. DELIVERABLE: Báo lỗi không đủ hàng
                        throw new Exception($"Không đủ tồn kho cho '{part.Name}'. Yêu cầu: {inputPart.Quantity}, Chỉ còn: {storageItem.Quantity}. Vui lòng báo cáo Admin.");
                    }

                    // 3. DELIVERABLE: Trừ kho
                    storageItem.Quantity -= inputPart.Quantity;
                    _context.Storages.Update(storageItem);
                    // === KẾT THÚC LOGIC TASK 2.7 ===


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

                    // 4. DELIVERABLE: Kiểm tra ngưỡng và cảnh báo Admin
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

                // DELIVERABLE: Lưu tất cả thay đổi
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["Message"] = "Đã ghi nhận phụ tùng, trừ kho và gửi báo giá thành công cho Staff duyệt.";
                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                // DELIVERABLE: Rollback nếu có lỗi
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
                await _jobService.CompleteJobAsync(id, TechnicianNote);
                TempData["Message"] = " Công việc đã được đánh dấu hoàn thành.";

                try
                {
                    // 2. Lấy thông tin đơn hàng và người dùng
                    var completedJob = await _context.OrderServices
                        .Include(o => o.User) // Lấy thông tin Account (Customer)
                        .Include(o => o.OrderDetails)
                            .ThenInclude(od => od.Service) // Lấy thông tin Dịch vụ
                        .FirstOrDefaultAsync(o => o.OrderId == id);

                    if (completedJob != null && completedJob.User != null)
                    {
                        var userAccount = completedJob.User;
                        var serviceNames = string.Join(", ", completedJob.OrderDetails.Select(s => s.Service?.Name));

                        var subject = "Thông báo: Dịch vụ xe của bạn đã hoàn tất";
                        var message = $@"
                    <p>Chào {userAccount.FullName},</p>
                    <p>Chúng tôi vui mừng thông báo rằng công việc bảo dưỡng/sửa chữa cho xe của bạn đã <strong>hoàn tất</strong>.</p>
                    <ul>
                        <li><strong>Mã lịch hẹn:</strong> {completedJob.OrderId}</li>
                        <li><strong>Dịch vụ:</strong> {serviceNames}</li>
                        <li><strong>Tổng chi phí:</strong> {completedJob.TotalCost:N0} đ</li>
                        <li><strong>Trạng thái:</strong> Đã hoàn thành</li>
                    </ul>
                    <p>Bạn có thể đến trung tâm dịch vụ của chúng tôi để nhận lại xe.</p>
                    <p>Cảm ơn bạn đã sử dụng dịch vụ của EV Service Center.</p>
                    <p>Trân trọng,</p>";

                        await _emailSender.SendEmailAsync(userAccount.Email, subject, message);
                    }
                }
                catch (Exception ex_email)
                {
                    // Nếu gửi mail lỗi, không làm dừng chương trình, chỉ log lại
                    Console.WriteLine($"Lỗi gửi mail thông báo hoàn thành: {ex_email.Message}");
                    // Vẫn redirect thành công
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
                                       Text = $"{p.Name} ({p.Brand ?? "N/A"}) - {p.UnitPrice:N0} ?"
                                   }).ToListAsync();
        }
    }
}