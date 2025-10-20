using EVCenterService.Models;
using EVCenterService.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace EVCenterService.Pages.Customer.Vehicles
{
    [Authorize(Roles = "Customer")]
    public class DeleteModel : PageModel
    {
        private readonly IVehicleService _vehicleService;

        public DeleteModel(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        [BindProperty]
        public Vehicle Vehicle { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            try
            {
                Vehicle = await _vehicleService.GetVehicleByIdAsync(id, userId);
                if (Vehicle == null)
                    return RedirectToPage("Index");
            }
            catch
            {
                return RedirectToPage("Index");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _vehicleService.DeleteVehicleAsync(id, userId);
            return RedirectToPage("Index");
        }
    }
}
