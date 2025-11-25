using EVCenterService.Models;
using EVCenterService.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace EVCenterService.Pages.Customer.Vehicles
{
    [Authorize(Roles = "Customer")]
    public class CreateModel : PageModel
    {
        private readonly IVehicleService _vehicleService;

        [BindProperty]
        public Vehicle Vehicle { get; set; }

        public CreateModel(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            if (await _vehicleService.IsVinDuplicateAsync(Vehicle.Vin))
            {
                ModelState.AddModelError("Vehicle.Vin", $"Số VIN '{Vehicle.Vin}' đã tồn tại trong hệ thống.");
                return Page();
            }

            Vehicle.UserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _vehicleService.AddVehicleAsync(Vehicle);

            return RedirectToPage("Index");
        }
    }
}
