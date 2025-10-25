using EVCenterService.Models;
using EVCenterService.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace EVCenterService.Pages.Technician.Jobs
{
    [Authorize(Roles = "Technician")]
    public class IndexModel : PageModel
    {
        private readonly ITechnicianJobService _jobService;

        public IndexModel(ITechnicianJobService jobService) => _jobService = jobService;
        public List<OrderService> AssignedJobs { get; set; } = new();

        public async Task OnGetAsync()
        {
            var techId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            AssignedJobs = await _jobService.GetAssignedJobsAsync(techId);
        }
    }
}
