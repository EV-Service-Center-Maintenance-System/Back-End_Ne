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
            PendingAppointments = (await _staffService.GetPendingAppointmentsAsync()).ToList();

            var statusesToReview = new[] { "PendingQuote" };

            ReadyToFinalizeAppointments = await _context.OrderServices
                .Include(o => o.Vehicle)
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Service)
                .Where(o => statusesToReview.Contains(o.Status))
                .OrderBy(o => o.AppointmentDate)
                .AsNoTracking()
                .ToListAsync();

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

        //public async Task<IActionResult> OnPostConfirmAsync(int id)
        //{
        //    await _staffService.ConfirmAppointmentAsync(id);
        //    TempData["Message"] = "✅ Appointment confirmed.";
        //    return RedirectToPage();
        //}

        //public async Task<IActionResult> OnPostRejectAsync(int id)
        //{
        //    await _staffService.RejectAppointmentAsync(id);
        //    TempData["Message"] = "❌ Appointment rejected.";
        //    return RedirectToPage();
        //}

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