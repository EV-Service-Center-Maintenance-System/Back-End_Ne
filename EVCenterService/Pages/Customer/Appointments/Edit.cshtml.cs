using EVCenterService.Data;
using EVCenterService.Models;
using EVCenterService.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EVCenterService.Pages.Customer.Appointments
{
    [Authorize(Roles = "Customer")]

    public class EditModel : PageModel
    {
        private readonly EVServiceCenterContext _context;
        private readonly ICustomerBookingService _service;

        public EditModel(EVServiceCenterContext context, ICustomerBookingService service)
        {
            _context = context;
            _service = service;
        }

        [BindProperty] public OrderService Booking { get; set; }
        [BindProperty] public int SelectedServiceId { get; set; }

        public SelectList VehicleList { get; set; }
        public SelectList ServiceList { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            Booking = await _service.GetBookingByIdAsync(id, userId);
            if (Booking == null) return NotFound();

            VehicleList = new SelectList(await _context.Vehicles.Where(v => v.UserId == userId).ToListAsync(), "VehicleId", "Model");
            ServiceList = new SelectList(await _context.ServiceCatalogs.ToListAsync(), "ServiceId", "Name");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _service.UpdateBookingAsync(Booking, SelectedServiceId);
            return RedirectToPage("Index");
        }
    }
}
