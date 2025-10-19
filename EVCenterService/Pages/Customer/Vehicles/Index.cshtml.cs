using EVCenterService.Models;
using EVCenterService.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace EVCenterService.Pages.Customer.Vehicles
{
    [Authorize(Roles = "Customer")]
    public class IndexModel : PageModel
    {
        private readonly IVehicleService _vehicleService;

        public IndexModel(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        public List<Vehicle> VehicleList { get; set; } = new();

        public async Task OnGetAsync()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            VehicleList = await _vehicleService.GetVehiclesForCustomerAsync(userId);
        }
    }
}
