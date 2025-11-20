using EVCenterService.Data;
using EVCenterService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic; 
using System.Linq; 
using System.Threading.Tasks;

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

        public List<SelectListItem> Centers { get; set; } = new();

        public class PartViewModel
        {
            public int PartId { get; set; }
            public string Name { get; set; }
            public string? Type { get; set; }
            public string? Brand { get; set; }
            public decimal? UnitPrice { get; set; }
            public int TotalQuantity { get; set; }
            public int CurrentTotalMinThreshold { get; set; }
            public int SuggestedTotalMinThreshold { get; set; }

        }


        public async Task OnGetAsync()
        {
            var date90DaysAgo = DateTime.Now.AddDays(-90);
            var numCenters = await _context.MaintenanceCenters.CountAsync();
            if (numCenters == 0) numCenters = 1; // Tránh chia cho 0

            var allParts = await _context.Parts.ToListAsync();

            var allStorageData = await _context.Storages
                .GroupBy(s => s.PartId)
                .Select(g => new
                {
                    PartId = g.Key,
                    TotalQuantity = g.Sum(s => s.Quantity),
                    CurrentTotalMinThreshold = g.Sum(s => s.MinThreshold)
                })
                .ToDictionaryAsync(x => x.PartId);

            var allUsageData = await _context.PartsUseds
                .Where(pu => pu.Order.AppointmentDate >= date90DaysAgo) 
                .GroupBy(pu => pu.PartId)
                .Select(g => new
                {
                    PartId = g.Key,
                    TotalUsedLast90Days = g.Sum(pu => pu.Quantity)
                })
                .ToDictionaryAsync(x => x.PartId);

            PartList = allParts.Select(p =>
            {
                allStorageData.TryGetValue(p.PartId, out var storage);
                var totalQuantity = storage?.TotalQuantity ?? 0;
                var currentTotalMinThreshold = storage?.CurrentTotalMinThreshold ?? 0;

                allUsageData.TryGetValue(p.PartId, out var usage);
                var totalUsedLast90Days = usage?.TotalUsedLast90Days ?? 0;

                var averageMonthlyDemandTotal = totalUsedLast90Days / 3.0;

                var averageMonthlyDemandPerCenter = averageMonthlyDemandTotal / numCenters;

                var safetyStockFactor = 1.5;

                var suggestedThresholdPerCenter = (int)Math.Ceiling(averageMonthlyDemandPerCenter * safetyStockFactor);

                if (suggestedThresholdPerCenter < 2) suggestedThresholdPerCenter = 2;

                var suggestedTotalMinThreshold = suggestedThresholdPerCenter * numCenters;


                return new PartViewModel
                {
                    PartId = p.PartId,
                    Name = p.Name,
                    Type = p.Type,
                    Brand = p.Brand,
                    UnitPrice = p.UnitPrice,
                    TotalQuantity = totalQuantity,
                    CurrentTotalMinThreshold = currentTotalMinThreshold,
                    SuggestedTotalMinThreshold = suggestedTotalMinThreshold
                };
            })
            .OrderBy(p => p.Name)
            .ToList();

            Centers = await _context.MaintenanceCenters
                .Select(c => new SelectListItem { Value = c.CenterId.ToString(), Text = c.Name })
                .ToListAsync();
        }

        // Thêm hàm này vào class IndexModel
        public async Task<IActionResult> OnPostRefillAsync(int PartId, int CenterId, int Quantity)
        {
            if (Quantity <= 0) return RedirectToPage();

            var storage = await _context.Storages
                .FirstOrDefaultAsync(s => s.PartId == PartId && s.CenterId == CenterId);

            if (storage == null)
            {
                // Nếu chưa có bản ghi kho, tạo mới
                storage = new Storage
                {
                    PartId = PartId,
                    CenterId = CenterId,
                    Quantity = Quantity,
                    MinThreshold = 5 // Mặc định
                };
                _context.Storages.Add(storage);
            }
            else
            {
                // Cộng dồn số lượng
                storage.Quantity += Quantity;
                _context.Storages.Update(storage);
            }

            await _context.SaveChangesAsync();

            TempData["StatusMessage"] = $"Đã nhập thêm {Quantity} cái vào kho thành công.";
            return RedirectToPage();
        }
    }
}