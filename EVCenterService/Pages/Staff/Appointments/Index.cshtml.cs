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
    [Authorize(Roles = "Staff")]
    public class IndexModel : PageModel
    {
        private readonly IStaffAppointmentService _staffService;
        private readonly EVServiceCenterContext _context;

        public IndexModel(IStaffAppointmentService staffService, EVServiceCenterContext context)
        {
            _staffService = staffService;
            _context = context;
        }

        public List<OrderService> PendingApprovalAppointments { get; set; } = new();
        public List<OrderService> PendingAssignmentAppointments { get; set; } = new();
        public List<OrderService> ReadyToFinalizeAppointments { get; set; } = new();

        // THAY ĐỔI 1: Bỏ 'TechnicianList' cũ và thay bằng 'AvailableTechniciansMap'
        // Đây là một "bản đồ" liên kết một OrderId (key) với danh sách KTV (value)
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
                .Where(o => o.Status == "Pending")
                .OrderBy(o => o.AppointmentDate)
                .ToListAsync();

            PendingAssignmentAppointments = await baseQuery
                .Where(o => o.Status == "Confirmed")
                .OrderBy(o => o.AppointmentDate)
                .ToListAsync();

            ReadyToFinalizeAppointments = await baseQuery
                .Where(o => o.Status == "PendingQuote")
                .OrderBy(o => o.AppointmentDate)
                .ToListAsync();

            // === THAY ĐỔI 2: Xây dựng logic lọc KTV ===

            // 1. Tạo bản đồ (map) logic nghiệp vụ: Dịch vụ nào -> Cần chứng chỉ đó
            // Tên dịch vụ và chứng chỉ này PHẢI KHỚP VỚI DATABASE CỦA BẠN
            var serviceCertMap = new Dictionary<string, string>
            {
                { "Battery Replacement", "Battery System Certified" }, 
                { "Brake Check", "Brake System Certified" },
                { "Cooling System Check", "Thermal & Cooling System Certified" }, 
                { "General Inspection", "General Inspection Certified" }
            };

            // 2. Tải TẤT CẢ KTV đang "Active" và chứng chỉ của họ
            var allTechnicians = await _context.Accounts
                .AsNoTracking()
                .Where(a => a.Role == "Technician" && a.Status == "Active")
                .Select(a => new
                {
                    a.UserId,
                    a.FullName,
                    a.Certification
                })
                .ToListAsync();

            // 3. Lọc KTV cho TỪNG đơn hàng trong tab "Cần phân công"
            foreach (var order in PendingAssignmentAppointments)
            {
                // 3.1. Tìm tất cả chứng chỉ CẦN CÓ cho đơn hàng này
                var requiredCerts = order.OrderDetails
                    .Select(od => serviceCertMap.GetValueOrDefault(od.Service.Name))
                    .Where(cert => cert != null) 
                    .Distinct()
                    .ToList();

                List<SelectListItem> availableTechList;

                if (requiredCerts.Any())
                {
                    availableTechList = allTechnicians
                        .Where(tech => !string.IsNullOrWhiteSpace(tech.Certification)
                                       && requiredCerts.Contains(tech.Certification))
                        .Select(tech => new SelectListItem { Value = tech.UserId.ToString(), Text = tech.FullName })
                        .ToList();
                }
                else
                {
                    availableTechList = allTechnicians
                        .Select(tech => new SelectListItem { Value = tech.UserId.ToString(), Text = tech.FullName })
                        .ToList();
                }

                AvailableTechniciansMap[order.OrderId] = availableTechList;
            }
        }

        public async Task<IActionResult> OnPostConfirmAsync(int id)
        {
            await _staffService.ConfirmAppointmentAsync(id);
            TempData["Message"] = "✅ Đã xác nhận lịch hẹn.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRejectAsync(int id)
        {
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
                await _staffService.AssignTechnicianAsync(id, technicianId);
                TempData["Message"] = "👷 Phân công kỹ thuật viên thành công.";
            }
            catch (Exception ex)
            {
                TempData["Message"] = $"Error: {ex.Message}";
            }
            return RedirectToPage();
        }

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
    }
}