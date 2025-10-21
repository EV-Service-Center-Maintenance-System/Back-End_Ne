using EVCenterService.Data;
using EVCenterService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EVCenterService.Pages.Admin.Parts
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly EVServiceCenterContext _context;

        public IndexModel(EVServiceCenterContext context)
        {
            _context = context;
        }

        public IList<PartViewModel> PartList { get; set; } = new List<PartViewModel>();

        public class PartViewModel
        {
            public int PartId { get; set; }
            public string Name { get; set; }
            public string? Type { get; set; }
            public string? Brand { get; set; }
            public decimal? UnitPrice { get; set; }
            public int TotalQuantity { get; set; } // T?ng t?n kho
            // Có th? thêm MinThreshold n?u c?n hi?n th? ph?c t?p h?n
        }


        public async Task OnGetAsync()
        {
            PartList = await _context.Parts
                .Select(p => new PartViewModel
                {
                    PartId = p.PartId,
                    Name = p.Name,
                    Type = p.Type,
                    Brand = p.Brand,
                    UnitPrice = p.UnitPrice,
                    // Tính t?ng s? l??ng t? b?ng Storage cho PartID này
                    TotalQuantity = _context.Storages
                                        .Where(s => s.PartId == p.PartId)
                                        .Sum(s => s.Quantity)
                })
                .OrderBy(p => p.Name)
                .ToListAsync();
        }
    }
}