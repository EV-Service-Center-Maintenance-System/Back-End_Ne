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
    public class DetailsModel : PageModel
    {
        private readonly IStaffAppointmentService _staffService;
        private readonly EVServiceCenterContext _context;

        public DetailsModel(IStaffAppointmentService staffService, EVServiceCenterContext context)
        {
            _staffService = staffService;
            _context = context;
        }

        public OrderService Appointment { get; set; }

        public List<SelectListItem> TechnicianList { get; set; } = new();

        [BindProperty]
        public Guid SelectedTechnicianId { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Appointment = await _staffService.GetAppointmentWithDetailsAsync(id);
            if (Appointment == null)
                return NotFound();

            TechnicianList = await _context.Accounts
                .Where(a => a.Role == "Technician")
                .Select(a => new SelectListItem
                {
                    Value = a.UserId.ToString(),
                    Text = a.FullName
                })
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostConfirmAsync(int id)
        {
            await _staffService.ConfirmAppointmentAsync(id);
            TempData["Message"] = "✅ Appointment confirmed successfully.";
            return RedirectToPage("Manage");
        }

        public async Task<IActionResult> OnPostRejectAsync(int id)
        {
            await _staffService.RejectAppointmentAsync(id);
            TempData["Message"] = "❌ Appointment rejected.";
            return RedirectToPage("Manage");
        }

        public async Task<IActionResult> OnPostAssignAsync(int id, Guid technicianId)
        {
            if (technicianId == Guid.Empty)
            {
                TempData["Message"] = "⚠️ Please select a technician before assigning.";
                return RedirectToPage(new { id });
            }

            await _staffService.AssignTechnicianAsync(id, technicianId);
            TempData["Message"] = "👷 Technician assigned successfully.";
            return RedirectToPage("Manage");
        }
    }
}
