using EVCenterService.Data;
using EVCenterService.Models;
using EVCenterService.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVCenterService.Pages.Staff.Appointments
{
    [Authorize(Roles = "Staff, Admin")] // Cho phép cả Admin
    public class IndexModel : PageModel
    {
        private readonly IStaffAppointmentService _staffService;
        private readonly EVServiceCenterContext _context;
        private readonly IEmailSender _emailSender;

        public IndexModel(IStaffAppointmentService staffService, EVServiceCenterContext context, IEmailSender emailSender)
        {
            _staffService = staffService;
            _context = context;
            _emailSender = emailSender;
        }

        public List<OrderService> PendingApprovalAppointments { get; set; } = new();
        public List<OrderService> PendingAssignmentAppointments { get; set; } = new();
        public List<OrderService> ReadyToFinalizeAppointments { get; set; } = new();
        public List<OrderService> CompletedForPickupOrders { get; set; } = new();

        public Dictionary<int, List<SelectListItem>> AvailableTechniciansMap { get; set; } = new();

        public async Task OnGetAsync()
        {
            // === PHẦN 1: Tải các đơn hàng (Giữ nguyên) ===
            var baseQuery = _context.OrderServices
                .Include(o => o.Vehicle)
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Service)
                .AsNoTracking();

            PendingApprovalAppointments = await baseQuery
                .Where(o => o.Status.ToLower() == "pending")
                .OrderBy(o => o.AppointmentDate)
                .ToListAsync();

            PendingAssignmentAppointments = await baseQuery
                .Where(o => o.Status.ToLower() == "confirmed")
                .OrderBy(o => o.AppointmentDate)
                .ToListAsync();

            CompletedForPickupOrders = await baseQuery
                .Where(o => o.Status == "TechnicianCompleted")
                .OrderByDescending(o => o.AppointmentDate)
                .ToListAsync();

            ReadyToFinalizeAppointments = await baseQuery
                .Where(o => o.Status.ToLower() == "pendingquote")
                .OrderBy(o => o.AppointmentDate)
                .ToListAsync();


            // === PHẦN 2: XÂY DỰNG LOGIC LỌC (GIẢI PHÁP B) ===

            // 1. Bản đồ logic: Dịch vụ nào -> Cần chứng chỉ đó
            // Tên phải khớp 100% với CSDL
            var serviceCertMap = new Dictionary<string, string>
            {
                { "Battery Replacement", "Battery System Certified" }, //
                { "Brake Check", "Brake System Certified" }, //
                { "Cooling System Check", "Thermal & Cooling System Certified" }, //
                { "General Inspection", "General Inspection Certified" } //
            };
            // (Lưu ý: Bạn phải đảm bảo tên chuỗi ở đây khớp với CSDL của bạn)

            // 2. Tải TẤT CẢ KTV đang "Active"
            var allTechnicians = await _context.Accounts
                .AsNoTracking()
                .Where(a => a.Role == "Technician" && a.Status == "Active")
                .Select(a => new { a.UserId, a.FullName, a.Certification })
                .ToListAsync();

            // 3. Lọc KTV cho TỪNG đơn hàng trong tab "Cần phân công"
            foreach (var order in PendingAssignmentAppointments)
            {
                // 3.1. Tìm tất cả chứng chỉ CẦN CÓ cho đơn hàng này
                var requiredCerts = order.OrderDetails
                    .Select(od => serviceCertMap.GetValueOrDefault(od.Service.Name))
                    .Where(cert => cert != null) // Lọc ra các dịch vụ không cần cert
                    .Distinct()
                    .ToList();

                // === LOGIC MỚI: Bỏ qua "General Inspection" NẾU có dịch vụ khác ===
                var generalCert = "General Inspection Certified";
                bool needsGeneral = requiredCerts.Contains(generalCert);

                // Nếu đơn hàng cần "General" VÀ một dịch vụ sửa chữa khác
                if (needsGeneral && requiredCerts.Count > 1)
                {
                    // Bỏ qua "General", chỉ yêu cầu cert sửa chữa
                    requiredCerts.Remove(generalCert);
                }
                // (Nếu chỉ có "General", requiredCerts vẫn là ["General Inspection Certified"])
                // (Nếu "Brake" + "Battery", requiredCerts vẫn là ["Brake", "Battery"])

                List<SelectListItem> availableTechList;

                // ===== BẮT ĐẦU LOGIC GIẢI PHÁP B =====

                if (requiredCerts.Any())
                {
                    // KỊCH BẢN 1: Đơn hàng CẦN chứng chỉ
                    // Tìm các KTV có CHỨA TẤT CẢ các chứng chỉ yêu cầu
                    availableTechList = allTechnicians
                        .Where(tech => {
                            if (string.IsNullOrEmpty(tech.Certification))
                                return false;

                            // Tách các chứng chỉ của KTV (ví dụ: "CertA;CertB")
                            var techCerts = tech.Certification.Split(';');

                            // Kiểm tra xem KTV này có TẤT CẢ (All) các chứng chỉ yêu cầu không
                            return requiredCerts.All(reqCert =>
                                techCerts.Contains(reqCert, StringComparer.OrdinalIgnoreCase));
                        })
                        .Select(tech => new SelectListItem { Value = tech.UserId.ToString(), Text = tech.FullName })
                        .ToList();
                }
                else
                {
                    // KỊCH BẢN 2: Không cần chứng chỉ (ví dụ: rửa xe,...)
                    availableTechList = allTechnicians
                        .Select(tech => new SelectListItem { Value = tech.UserId.ToString(), Text = tech.FullName })
                        .ToList();
                }

                // 3.2. Lưu danh sách KTV đã lọc vào bản đồ
                AvailableTechniciansMap[order.OrderId] = availableTechList;
            }
        }

        // === PHẦN 3: CÁC HÀM POST (Giữ nguyên) ===
        // Các hàm này không thay đổi vì chúng chỉ nhận 1 OrderID và (nếu cần) 1 TechnicianID

        // XỬ LÝ CHO TAB 1: Chờ duyệt đơn
        public async Task<IActionResult> OnPostConfirmAsync(int id)
        {
            await _staffService.ConfirmAppointmentAsync(id);
            TempData["Message"] = "✅ Đã xác nhận lịch hẹn.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRejectAsync(int id)
        {
            // (Bạn nên thêm logic gửi email cho khách ở đây)
            await _staffService.RejectAppointmentAsync(id);
            TempData["Message"] = "❌ Đã từ chối lịch hẹn.";
            return RedirectToPage();
        }

        // XỬ LÝ CHO TAB 2: Cần phân công
        public async Task<IActionResult> OnPostAssignAsync(int id, Guid technicianId)
        {
            if (technicianId == Guid.Empty)
            {
                TempData["Message"] = "⚠️ Vui lòng chọn kỹ thuật viên trước khi phân công.";
                return RedirectToPage();
            }

            try
            {
                // Hàm này đã có logic kiểm tra trùng lịch KTV
                await _staffService.AssignTechnicianAsync(id, technicianId);
                TempData["Message"] = "👷 Phân công kỹ thuật viên thành công.";
            }
            catch (Exception ex)
            {
                // Bắt lỗi trùng lịch KTV
                TempData["Message"] = $"Error: {ex.Message}";
            }
            return RedirectToPage();
        }

        // XỬ LÝ HỦY LỊCH (Đã thêm ở lượt trước)
        public async Task<IActionResult> OnPostCancelAsync(int id, string cancellationReason)
        {
            if (string.IsNullOrWhiteSpace(cancellationReason))
            {
                TempData["Message"] = "⚠️ Bạn phải nhập lý do hủy lịch.";
                return RedirectToPage();
            }

            try
            {
                await _staffService.CancelAppointmentByStaffAsync(id, cancellationReason);
                TempData["Message"] = "✅ Đã hủy lịch hẹn và gửi email thông báo cho khách hàng.";
            }
            catch (Exception ex)
            {
                TempData["Message"] = $"Error: {ex.Message}";
            }
            return RedirectToPage();
        }

        // Handler cho nút "Xác nhận đã lấy"
        public async Task<IActionResult> OnPostConfirmPickupAsync(int id)
        {
            var order = await _context.OrderServices
                .Include(o => o.User)
                .Include(o => o.Vehicle) // Phải Include Vehicle
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null || order.User == null)
                return NotFound();

            // 1. Đổi trạng thái Order -> "PickedUp" (đã lấy)
            order.Status = "PickedUp";
            _context.OrderServices.Update(order);
            await _context.SaveChangesAsync();

            // 2. Gửi Email Cảm ơn & Hẹn lần sau
            try
            {
                var vehicle = order.Vehicle;
                string maintenanceSchedule = "";

                if (vehicle != null)
                {
                    var currentMileage = vehicle.Mileage ?? 0;
                    var nextMileage = currentMileage + 5000;
                    var lastMaintenance = (vehicle.LastMaintenanceDate ?? DateOnly.FromDateTime(DateTime.Now));
                    var nextDate = lastMaintenance.AddMonths(6);

                    maintenanceSchedule = $@"
                        <p>Dựa trên lần bảo dưỡng này (Số Km: {currentMileage:N0} km), chúng tôi khuyến nghị lần bảo dưỡng tiếp theo của bạn là:</p>
                        <ul>
                            <li><strong>Theo số Km:</strong> {nextMileage:N0} km</li>
                            <li><strong>Theo thời gian:</strong> {nextDate:dd/MM/yyyy}</li>
                        </ul>";
                }

                var subject = "Cảm ơn bạn đã sử dụng dịch vụ tại EV Service Center!";
                var message = $@"
                    <p>Chào {order.User.FullName},</p>
                    <p>Chúng tôi xác nhận bạn đã nhận xe (Đơn hàng #{order.OrderId}) thành công. Cảm ơn bạn đã tin tưởng dịch vụ của chúng tôi.</p>
                    {maintenanceSchedule}
                    <p>Rất mong được phục vụ bạn trong lần tiếp theo.</p>
                    <p>Trân trọng,</p>";

                await _emailSender.SendEmailAsync(order.User.Email, subject, message);
                TempData["Message"] = "Đã xác nhận giao xe thành công và gửi email Cảm ơn.";
            }
            catch (Exception ex)
            {
                TempData["Message"] = $"Đã xác nhận giao xe, nhưng gửi email thất bại: {ex.Message}";
            }

            return RedirectToPage();
        }

        // Handler cho nút "!" (Nhắc nhở)
        public async Task<IActionResult> OnPostSendReminderAsync(int id)
        {
            var order = await _context.OrderServices
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null || order.User == null)
                return NotFound();

            try
            {
                var subject = "Thông báo: Nhắc nhở nhận xe tại EV Service Center";
                var message = $@"
                    <p>Chào {order.User.FullName},</p>
                    <p>Chúng tôi thông báo xe của bạn (Đơn hàng #{order.OrderId}) đã hoàn tất dịch vụ và đang chờ bạn đến nhận tại trung tâm.</p>
                    <p>Vui lòng sắp xếp thời gian đến nhận xe sớm nhất.</p>
                    <p>Trân trọng,</p>";

                await _emailSender.SendEmailAsync(order.User.Email, subject, message);
                TempData["Message"] = $"Đã gửi email nhắc nhở đến {order.User.Email} thành công.";
            }
            catch (Exception ex)
            {
                TempData["Message"] = $"Gửi email nhắc nhở thất bại: {ex.Message}";
            }

            return RedirectToPage();
        }
    }
}