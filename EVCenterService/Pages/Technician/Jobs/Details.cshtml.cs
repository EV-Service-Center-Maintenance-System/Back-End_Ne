using EVCenterService.Data;
using EVCenterService.Models;
using EVCenterService.Pages.Staff.Appointments;
using EVCenterService.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace EVCenterService.Pages.Technician.Jobs
{
    [Authorize(Roles = "Technician")]
    public class DetailsModel : PageModel
    {
        private readonly ITechnicianJobService _jobService;
        private readonly EVServiceCenterContext _context;

        public DetailsModel(ITechnicianJobService jobService, EVServiceCenterContext context)
        {
            _jobService = jobService;
            _context = context;
        }

        public OrderService Job { get; set; } = default!;

        [BindProperty]
        public string? TechnicianNote { get; set; }

        public decimal ServiceTotalCost { get; set; }
        public List<SelectListItem> AvailableParts { get; set; } = new();

        [BindProperty]
        public List<PartUsageInput> PartsUsedInput { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Job = await _jobService.GetJobDetailAsync(id);
            if (Job == null)
                return NotFound();

            if (Job.Status == "InProgress")
            {
                ServiceTotalCost = Job.OrderDetails.Sum(od => (od.UnitPrice ?? 0) * (od.Quantity ?? 1));
                await LoadAvailableParts();

                PartsUsedInput = Job.PartsUseds
                    .Where(pu => pu.PartId.HasValue) 
                    .Select(pu => new PartUsageInput
                    {
                        PartId = pu.PartId.Value, 
                        Quantity = pu.Quantity,
                        Note = pu.Note
                    }).ToList();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostSubmitQuoteAsync(int id)
        {
            // Tải lại Job với các chi tiết cần thiết
            Job = await _context.OrderServices
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (Job == null) return NotFound();

            var reloadDataOnError = async () =>
            {
                ServiceTotalCost = Job.OrderDetails.Sum(od => (od.UnitPrice ?? 0) * (od.Quantity ?? 1));
                await LoadAvailableParts();
            };

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                decimal totalPartsCost = 0;

                var existingPartsUsed = await _context.PartsUseds.Where(pu => pu.OrderId == id).ToListAsync();
                if (existingPartsUsed.Any())
                {
                    _context.PartsUseds.RemoveRange(existingPartsUsed);
                }

                foreach (var inputPart in PartsUsedInput.Where(p => p.PartId > 0 && p.Quantity > 0))
                {
                    var part = await _context.Parts.FindAsync(inputPart.PartId);
                    if (part == null) throw new Exception($"Không tìm thấy phụ tùng ID {inputPart.PartId}");

                    var partsUsedEntry = new PartsUsed
                    {
                        OrderId = id,
                        PartId = inputPart.PartId,
                        Quantity = inputPart.Quantity, // <-- SỬA LỖI TYPO "inputTpart"
                        Note = inputPart.Note
                    };
                    _context.PartsUseds.Add(partsUsedEntry);

                    totalPartsCost += (part.UnitPrice ?? 0) * inputPart.Quantity;
                }

                decimal serviceCost = Job.OrderDetails.Sum(od => (od.UnitPrice ?? 0) * (od.Quantity ?? 1));
                Job.TotalCost = serviceCost + totalPartsCost;
                Job.Status = "PendingQuote";

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["Message"] = "Đã gửi báo giá thành công cho Staff duyệt.";
                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError(string.Empty, $"Lỗi khi gửi báo giá: {ex.Message}");
                await reloadDataOnError();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostCompleteAsync(int id)
        {
            try
            {
                await _jobService.CompleteJobAsync(id, TechnicianNote);
                TempData["Message"] = " Công việc đã được đánh dấu hoàn thành.";
                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                Job = await _jobService.GetJobDetailAsync(id);
                return Page();
            }
        }

        public async Task<IActionResult> OnPostCancelAsync(int id)
        {
            await _jobService.CancelJobAsync(id, TechnicianNote);
            TempData["Message"] = " Công việc đã bị hủy.";
            return RedirectToPage("Index");
        }

        private async Task LoadAvailableParts()
        {
            AvailableParts = await _context.Parts
                                   .OrderBy(p => p.Name)
                                   .Select(p => new SelectListItem
                                   {
                                       Value = p.PartId.ToString(),
                                       Text = $"{p.Name} ({p.Brand ?? "N/A"}) - {p.UnitPrice:N0} ?"
                                   }).ToListAsync();
        }
    }
}