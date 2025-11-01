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

        // TH�M M?I: Danh s�ch ph? t�ng Tech ?� b�o gi�
        public List<PartsUsed> PartsToReview { get; set; } = new();

        // X�A: Kh�ng c?n AvailableParts v� PartsUsedInput n?a

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // T?i th�ng tin, BAO G?M ph? t�ng Tech ?� b�o gi�
            Appointment = await _context.OrderServices
                .Include(o => o.User)
                .Include(o => o.Vehicle)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Service)
                .Include(o => o.PartsUseds) // <--- TH�M M?I
                    .ThenInclude(pu => pu.Part) // <--- TH�M M?I
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (Appointment == null) return NotFound();

            if (Appointment.Status != "PendingQuote")
            {
                TempData["ErrorMessage"] = "B�o gi� n�y kh�ng ? tr?ng th�i ch? duy?t.";
                return RedirectToPage("./Index");
            }

            // L?y danh s�ch ph? t�ng Tech ?� b�o gi�
            PartsToReview = Appointment.PartsUseds.ToList();

            // T�nh ti?n d?ch v?
            ServiceTotalCost = Appointment.OrderDetails.Sum(od => (od.UnitPrice ?? 0) * (od.Quantity ?? 1));

            return Page();
        }

        // THAY TH? HO�N TO�N LOGIC OnPostAsync
        public async Task<IActionResult> OnPostAsync(int id)
        {
            Appointment = await _context.OrderServices
                .Include(o => o.PartsUseds) // T?i l?i ph? t�ng ?� b�o gi�
                .Include(o => o.OrderDetails) // T?i l?i ?? t�nh ServiceTotalCost
                    .ThenInclude(od => od.Service)
                .Include(o => o.User) // T?i l?i ?? hi?n th? n?u c� l?i
                .Include(o => o.Vehicle) // T?i l?i ?? hi?n th? n?u c� l?i
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (Appointment == null) return NotFound();

            // L?y CenterID (Gi?ng logic c? c?a b?n)
            var slot = await _context.Slots.FirstOrDefaultAsync(s => s.OrderId == id);
            var centerId = slot?.CenterId;

            if (centerId == null)
            {
                ModelState.AddModelError(string.Empty, $"Kh�ng th? x�c ??nh trung t�m d?ch v?. Kh�ng th? tr? kho.");
                return await ReloadPageDataOnErrorAsync(id); // T?i l?i d? li?u khi l?i
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. TR? KHO (Logic quan tr?ng)
                // L?p qua c�c ph? t�ng Tech ?� b�o gi� (?� l?u trong DB)
                foreach (var partUsed in Appointment.PartsUseds)
                {
                    var storageItem = await _context.Storages
                                            .FirstOrDefaultAsync(s => s.CenterId == centerId && s.PartId == partUsed.PartId);

                    if (storageItem == null)
                    {
                        var part = await _context.Parts.FindAsync(partUsed.PartId);
                        throw new Exception($"Kh�ng t�m th?y ph? t�ng '{part?.Name}' trong kho.");
                    }
                    if (storageItem.Quantity < partUsed.Quantity)
                    {
                        var part = await _context.Parts.FindAsync(partUsed.PartId);
                        throw new Exception($"Kh�ng ?? s? l??ng t?n kho cho '{part?.Name}'.");
                    }

                    storageItem.Quantity -= partUsed.Quantity; // Tr? kho
                }

                // 2. T?O H�A ??N (Invoice) - D�ng DB ?� s?a
                var newInvoice = new Invoice
                {
                    // Li�n k?t v?i OrderService (nh? b??c s?a DB)
                    OrderId = Appointment.OrderId,
                    Amount = Appointment.TotalCost,
                    Status = "Unpaid",
                    IssueDate = DateTime.Now,
                    DueDate = DateTime.Now.AddDays(7)
                };
                _context.Invoices.Add(newInvoice);

                // 3. C?p nh?t tr?ng th�i Order -> Ch? kh�ch thanh to�n
                Appointment.Status = "PendingPayment";

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["StatusMessage"] = $"?� duy?t b�o gi�, tr? kho v� t?o h�a ??n cho kh�ch h�ng.";

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError(string.Empty, $"L?i khi duy?t b�o gi�: {ex.Message}");
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