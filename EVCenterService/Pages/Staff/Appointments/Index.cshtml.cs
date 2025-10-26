using EVCenterService.Data;
using EVCenterService.Models;
using EVCenterService.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic; // Thêm
using System.Linq; // Thêm

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

        public List<OrderService> PendingAppointments { get; set; } = new();
        public List<OrderService> ReadyToFinalizeAppointments { get; set; } = new();
        public List<SelectListItem> TechnicianList { get; set; } = new();

        public async Task OnGetAsync()
        {
            // Tải dữ liệu cho Tab 1 (Manage) - Giữ nguyên
            PendingAppointments = (await _staffService.GetPendingAppointmentsAsync()).ToList();

            // SỬA TAB 2 (TRƯỚC LÀ FINALIZE)
            // Giờ đây nó sẽ tìm các báo giá "PendingQuote"
            var statusesToReview = new[] { "PendingQuote" }; // <-- SỬA Ở ĐÂY

            ReadyToFinalizeAppointments = await _context.OrderServices
                .Include(o => o.Vehicle)
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Service)
                .Where(o => statusesToReview.Contains(o.Status)) // <-- SỬA Ở ĐÂY
                .OrderBy(o => o.AppointmentDate)
                .AsNoTracking()
                .ToListAsync();

            // Tải danh sách KTV (giữ nguyên)
            TechnicianList = _context.Accounts
                .AsNoTracking()
                .Where(a => a.Role == "Technician")
                .Select(a => new SelectListItem
                {
                    Value = a.UserId.ToString(),
                    Text = a.FullName
                })
                .ToList();
        }

        // Các hàm OnPost... (giữ nguyên)
        public async Task<IActionResult> OnPostConfirmAsync(int id)
        {
            await _staffService.ConfirmAppointmentAsync(id);
            TempData["Message"] = "✅ Appointment confirmed.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRejectAsync(int id)
        {
            await _staffService.RejectAppointmentAsync(id);
            TempData["Message"] = "❌ Appointment rejected.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAssignAsync(int id, Guid technicianId)
        {
            if (technicianId == Guid.Empty)
            {
                TempData["Message"] = "⚠️ Vui lòng chọn kỹ thuật viên trước khi phân công.";
                return RedirectToPage();
            }

            await _staffService.AssignTechnicianAsync(id, technicianId);
            TempData["Message"] = "👷 Phân công kỹ thuật viên thành công.";
            return RedirectToPage();
        }
    }
}