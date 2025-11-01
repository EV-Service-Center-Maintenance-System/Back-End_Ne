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
    public class IndexModel : PageModel // Đổi tên class từ ManageModel (nếu có) thành IndexModel
    {
        private readonly IStaffAppointmentService _staffService;
        private readonly EVServiceCenterContext _context;

        public IndexModel(IStaffAppointmentService staffService, EVServiceCenterContext context)
        {
            _staffService = staffService;
            _context = context;
        }

        // SỬA: Chia làm 3 danh sách
        public List<OrderService> PendingApprovalAppointments { get; set; } = new();
        public List<OrderService> PendingAssignmentAppointments { get; set; } = new();
        public List<OrderService> ReadyToFinalizeAppointments { get; set; } = new();

        public List<SelectListItem> TechnicianList { get; set; } = new();

        public async Task OnGetAsync()
        {
            var baseQuery = _context.OrderServices
                .Include(o => o.Vehicle)
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Service)
                .AsNoTracking();

            // 1. YÊU CẦU MỚI: Tab "Chờ duyệt đơn"
            PendingApprovalAppointments = await baseQuery
                .Where(o => o.Status == "Pending")
                .OrderBy(o => o.AppointmentDate)
                .ToListAsync();

            // 2. Tab "Cần phân công"
            PendingAssignmentAppointments = await baseQuery
                .Where(o => o.Status == "Confirmed")
                .OrderBy(o => o.AppointmentDate)
                .ToListAsync();

            // 3. Tab "Chờ duyệt báo giá"
            ReadyToFinalizeAppointments = await baseQuery
                .Where(o => o.Status == "PendingQuote")
                .OrderBy(o => o.AppointmentDate)
                .ToListAsync();

            // Tải danh sách KTV
            TechnicianList = await _context.Accounts
                .AsNoTracking()
                .Where(a => a.Role == "Technician")
                .Select(a => new SelectListItem
                {
                    Value = a.UserId.ToString(),
                    Text = a.FullName
                })
                .ToListAsync();
        }

        // XỬ LÝ CHO TAB 1: Chờ duyệt đơn
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
    }
}