using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Test.Pages.Admin.Staff
{
    [Authorize(Roles = "Admin, Staff")]
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
