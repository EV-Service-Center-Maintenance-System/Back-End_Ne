using EVCenterService.Models;
using EVCenterService.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EVCenterService.Pages.Technician.Jobs
{
    [Authorize(Roles = "Technician")]
    public class DetailsModel : PageModel
    {
        private readonly ITechnicianJobService _jobService;

        public DetailsModel(ITechnicianJobService jobService) => _jobService = jobService;

        public OrderService Job { get; set; }

        [BindProperty]
        public string? TechnicianNote { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Job = await _jobService.GetJobDetailAsync(id);
            if (Job == null)
                return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostCompleteAsync(int id)
        {
            await _jobService.CompleteJobAsync(id, TechnicianNote);
            TempData["Message"] = " Công việc đã được đánh dấu hoàn thành.";
            return RedirectToPage("Index");
        }

        public async Task<IActionResult> OnPostCancelAsync(int id)
        {
            await _jobService.CancelJobAsync(id, TechnicianNote);
            TempData["Message"] = " Công việc đã bị hủy.";
            return RedirectToPage("Index");
        }
    }
}
