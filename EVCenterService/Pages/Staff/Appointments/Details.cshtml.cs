using EVCenterService.Models;
using EVCenterService.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EVCenterService.Pages.Staff.Appointments
{
    [Authorize(Roles = "Staff")]
    public class DetailsModel : PageModel
    {
        private readonly IStaffAppointmentService _staffService;

        public DetailsModel(IStaffAppointmentService staffService)
        {
            _staffService = staffService;
        }

        public OrderService Appointment { get; set; }

        [BindProperty]
        public Guid SelectedTechnicianId { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Appointment = await _staffService.GetAppointmentWithDetailsAsync(id);

            if (Appointment == null)
                return NotFound();

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
            await _staffService.AssignTechnicianAsync(id, technicianId);
            TempData["Message"] = "👷 Technician assigned successfully.";
            return RedirectToPage("Manage");
        }
    }
}
