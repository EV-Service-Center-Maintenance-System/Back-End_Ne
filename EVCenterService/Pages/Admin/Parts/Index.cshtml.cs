using EVCenterService.Data;
using EVCenterService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
            if (numCenters == 0) numCenters = 1; // Tr�nh chia cho 0

            // 1. L?y t?t c? Parts
            var allParts = await _context.Parts.ToListAsync();

            // 2. L?y t?t c? d? li?u Storage (gom nh�m theo PartId)
            var allStorageData = await _context.Storages
                .GroupBy(s => s.PartId)
                .Select(g => new
                {
                    PartId = g.Key,
                    TotalQuantity = g.Sum(s => s.Quantity),
                    CurrentTotalMinThreshold = g.Sum(s => s.MinThreshold)
                })
                .ToDictionaryAsync(x => x.PartId);

            // 3. L?y t?t c? d? li?u s? d?ng trong 90 ng�y (gom nh�m theo PartId)
            var allUsageData = await _context.PartsUseds
                .Where(pu => pu.Order.AppointmentDate >= date90DaysAgo) // L?c theo ng�y
                .GroupBy(pu => pu.PartId)
                .Select(g => new
                {
                    PartId = g.Key,
                    TotalUsedLast90Days = g.Sum(pu => pu.Quantity)
                })
                .ToDictionaryAsync(x => x.PartId);

            // 4. K?t h?p logic
            PartList = allParts.Select(p =>
            {
                // L?y d? li?u t?n kho
                allStorageData.TryGetValue(p.PartId, out var storage);
                var totalQuantity = storage?.TotalQuantity ?? 0;
                var currentTotalMinThreshold = storage?.CurrentTotalMinThreshold ?? 0;

                // L?y d? li?u s? d?ng
                allUsageData.TryGetValue(p.PartId, out var usage);
                var totalUsedLast90Days = usage?.TotalUsedLast90Days ?? 0;

                // ===== LOGIC G?I � (THAY TH? AI) =====
                // 1. Nhu c?u trung b�nh 30 ng�y (t?ng)
                var averageMonthlyDemandTotal = totalUsedLast90Days / 3.0;
                // 2. Nhu c?u trung b�nh 30 ng�y (cho m?i trung t�m)
                var averageMonthlyDemandPerCenter = averageMonthlyDemandTotal / numCenters;
                // 3. H? s? an to�n (VD: mu?n t?n kho ?? d�ng 1.5 th�ng)
                var safetyStockFactor = 1.5;
                // 4. Ng??ng t?i thi?u g?i � (cho m?i trung t�m)
                var suggestedThresholdPerCenter = (int)Math.Ceiling(averageMonthlyDemandPerCenter * safetyStockFactor);

                // Lu�n gi? �t nh?t 2 c�i (ng??ng t?i thi?u)
                if (suggestedThresholdPerCenter < 2) suggestedThresholdPerCenter = 2;

                // 5. T?ng ng??ng t?i thi?u g?i � (cho t?t c? trung t�m)
                var suggestedTotalMinThreshold = suggestedThresholdPerCenter * numCenters;
                // ===== K?T TH�C LOGIC =====

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
        }
    }
}