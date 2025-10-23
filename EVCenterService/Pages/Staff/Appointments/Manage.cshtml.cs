using EVCenterService.Models;
using EVCenterService.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EVCenterService.Pages.Staff.Appointments
{
    [Authorize(Roles = "Staff")]
    public class ManageModel : PageModel
    {
        private readonly IStaffAppointmentService _service;

        public ManageModel(IStaffAppointmentService service)
        {
            _service = service;
        }

        public List<OrderService> PendingAppointments { get; set; } = new();

        [BindProperty]
        public int SelectedOrderId { get; set; }

        [BindProperty]
        public Guid SelectedTechnicianId { get; set; }

        public async Task OnGetAsync()
        {
            PendingAppointments = (await _service.GetPendingAppointmentsAsync()).ToList();
        }

        public async Task<IActionResult> OnPostConfirmAsync(int orderId)
        {
            await _service.ConfirmAppointmentAsync(orderId);
            TempData["Message"] = $"✅ Confirm #{orderId}";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRejectAsync(int orderId)
        {
            await _service.RejectAppointmentAsync(orderId);
            TempData["Message"] = $"❌ Reject #{orderId}";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAssignAsync(int orderId, Guid technicianId)
        {
            await _service.AssignTechnicianAsync(orderId, technicianId);
            TempData["Message"] = $"👷‍♂️ Assigned for technican #{orderId}";
            return RedirectToPage();
        }
    }
}
