using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EVCenterService.Pages.Customer
{
    [Authorize(Roles = "Customer")]
    public class IndexModel : PageModel
    {
        public IActionResult OnGet()
        {
            return RedirectToPagePermanent("/Customer/Profile");
        }
    }
}