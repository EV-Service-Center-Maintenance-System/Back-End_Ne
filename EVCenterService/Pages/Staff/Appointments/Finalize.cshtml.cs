using EVCenterService.Data;
using EVCenterService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVCenterService.Pages.Staff.Appointments
{
    [Authorize(Roles = "Staff, Admin")]
    public class FinalizeModel : PageModel
    {
        private readonly EVServiceCenterContext _context;

        public FinalizeModel(EVServiceCenterContext context)
        {
            _context = context;
        }

        public OrderService Appointment { get; set; } = default!;
        public decimal ServiceTotalCost { get; set; }

        // THÊM M?I: Danh sách ph? tùng Tech ?ã báo giá
        public List<PartsUsed> PartsToReview { get; set; } = new();

        // XÓA: Không c?n AvailableParts và PartsUsedInput n?a

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // T?i thông tin, BAO G?M ph? tùng Tech ?ã báo giá
            Appointment = await _context.OrderServices
                .Include(o => o.User)
                .Include(o => o.Vehicle)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Service)
                .Include(o => o.PartsUseds) // <--- THÊM M?I
                    .ThenInclude(pu => pu.Part) // <--- THÊM M?I
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (Appointment == null) return NotFound();

            if (Appointment.Status != "PendingQuote")
            {
                TempData["ErrorMessage"] = "Báo giá này không ? tr?ng thái ch? duy?t.";
                return RedirectToPage("./Index");
            }

            // L?y danh sách ph? tùng Tech ?ã báo giá
            PartsToReview = Appointment.PartsUseds.ToList();

            // Tính ti?n d?ch v?
            ServiceTotalCost = Appointment.OrderDetails.Sum(od => (od.UnitPrice ?? 0) * (od.Quantity ?? 1));

            return Page();
        }

        // THAY TH? HOÀN TOÀN LOGIC OnPostAsync
        public async Task<IActionResult> OnPostAsync(int id)
        {
            Appointment = await _context.OrderServices
                .Include(o => o.PartsUseds) // T?i l?i ph? tùng ?ã báo giá
                .Include(o => o.OrderDetails) // T?i l?i ?? tính ServiceTotalCost
                    .ThenInclude(od => od.Service)
                .Include(o => o.User) // T?i l?i ?? hi?n th? n?u có l?i
                .Include(o => o.Vehicle) // T?i l?i ?? hi?n th? n?u có l?i
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (Appointment == null) return NotFound();

            // L?y CenterID (Gi?ng logic c? c?a b?n)
            var slot = await _context.Slots.FirstOrDefaultAsync(s => s.OrderId == id);
            var centerId = slot?.CenterId;

            if (centerId == null)
            {
                ModelState.AddModelError(string.Empty, $"Không th? xác ??nh trung tâm d?ch v?. Không th? tr? kho.");
                return await ReloadPageDataOnErrorAsync(id); // T?i l?i d? li?u khi l?i
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. TR? KHO (Logic quan tr?ng)
                // L?p qua các ph? tùng Tech ?ã báo giá (?ã l?u trong DB)
                foreach (var partUsed in Appointment.PartsUseds)
                {
                    var storageItem = await _context.Storages
                                            .FirstOrDefaultAsync(s => s.CenterId == centerId && s.PartId == partUsed.PartId);

                    if (storageItem == null)
                    {
                        var part = await _context.Parts.FindAsync(partUsed.PartId);
                        throw new Exception($"Không tìm th?y ph? tùng '{part?.Name}' trong kho.");
                    }
                    if (storageItem.Quantity < partUsed.Quantity)
                    {
                        var part = await _context.Parts.FindAsync(partUsed.PartId);
                        throw new Exception($"Không ?? s? l??ng t?n kho cho '{part?.Name}'.");
                    }

                    storageItem.Quantity -= partUsed.Quantity; // Tr? kho
                }

                // 2. T?O HÓA ??N (Invoice) - Dùng DB ?ã s?a
                var newInvoice = new Invoice
                {
                    // Liên k?t v?i OrderService (nh? b??c s?a DB)
                    OrderId = Appointment.OrderId,
                    Amount = Appointment.TotalCost,
                    Status = "Unpaid",
                    IssueDate = DateTime.Now,
                    DueDate = DateTime.Now.AddDays(7)
                };
                _context.Invoices.Add(newInvoice);

                // 3. C?p nh?t tr?ng thái Order -> Ch? khách thanh toán
                Appointment.Status = "PendingPayment";

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["StatusMessage"] = $"?ã duy?t báo giá, tr? kho và t?o hóa ??n cho khách hàng.";

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError(string.Empty, $"L?i khi duy?t báo giá: {ex.Message}");
                return await ReloadPageDataOnErrorAsync(id); 
            }
        }

        private async Task<IActionResult> ReloadPageDataOnErrorAsync(int id)
        {
            Appointment = await _context.OrderServices
                .Include(o => o.User)
                .Include(o => o.Vehicle)
                .Include(o => o.OrderDetails).ThenInclude(od => od.Service)
                .Include(o => o.PartsUseds).ThenInclude(pu => pu.Part)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (Appointment != null)
            {
                PartsToReview = Appointment.PartsUseds.ToList();
                ServiceTotalCost = Appointment.OrderDetails.Sum(od => (od.UnitPrice ?? 0) * (od.Quantity ?? 1));
            }

            return Page();
        }
    }
}