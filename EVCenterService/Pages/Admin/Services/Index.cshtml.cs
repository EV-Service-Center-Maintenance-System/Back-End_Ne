using EVCenterService.Data;
using EVCenterService.Models;
using EVCenterService.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EVCenterService.Pages.Admin.Services
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly IServiceCatalogService _service;

        public IndexModel(IServiceCatalogService service)
        {
            _service = service;
        }

        public IList<ServiceCatalog> ServiceList { get; set; } = new List<ServiceCatalog>();

        public async Task OnGetAsync()
        {
            ServiceList = await _service.GetAllServicesAsync();
        }
    }
}
