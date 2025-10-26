using EVCenterService.Data;
using EVCenterService.Models;
using EVCenterService.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EVCenterService.Pages.Staff.Appointments
{
    [Authorize(Roles = "Staff")]
    public class ManageModel : PageModel
    {
        private readonly IStaffAppointmentService _staffService;
        private readonly EVServiceCenterContext _context;

        public ManageModel(IStaffAppointmentService staffService, EVServiceCenterContext context)
        {
            _staffService = staffService;
            _context = context;
        }

        public List<OrderService> Appointments { get; set; } = new();
        public List<SelectListItem> TechnicianList { get; set; } = new();

        public async Task OnGetAsync()
        {
            Appointments = (await _staffService.GetPendingAppointmentsAsync()).ToList();

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
