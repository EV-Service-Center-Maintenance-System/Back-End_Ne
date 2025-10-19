using EVCenterService.Models;
using EVCenterService.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace EVCenterService.Pages.Customer.Vehicles
{
    [Authorize(Roles = "Customer")]
    public class EditModel : PageModel
    {
        private readonly IVehicleService _vehicleService;

        [BindProperty]
        public Vehicle Vehicle { get; set; }

        public EditModel(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            Vehicle = await _vehicleService.GetVehicleByIdAsync(id, userId);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _vehicleService.UpdateVehicleAsync(Vehicle, userId);

            return RedirectToPage("Index");
        }
    }
}
