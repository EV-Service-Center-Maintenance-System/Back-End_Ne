using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EVCenterService.Pages.Staff
{
    [Authorize(Roles = "Staff")]
    public class IndexModel : PageModel
    {
        public IActionResult OnGet()
        {
            return RedirectToPage("/Staff/Appointments/Index");
        }
    }
}
