using EVCenterService.Data;
using EVCenterService.Models;
using EVCenterService.Service.Interfaces;
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
        private readonly IEmailSender _emailSender;

        public FinalizeModel(EVServiceCenterContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
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
                TempData["ErrorMessage"] = "Báo giá này không ở trạng thái chờ duy?t.";
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
                ModelState.AddModelError(string.Empty, $"Không thể xác minh trung tâm dịch vụ. Không thể trừ kho.");
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
                        throw new Exception($"Không tìm thấy phụ tùng '{part?.Name}' trong kho.");
                    }
                    if (storageItem.Quantity < partUsed.Quantity)
                    {
                        var part = await _context.Parts.FindAsync(partUsed.PartId);
                        throw new Exception($"Không đủ số lượng tồn kho cho '{part?.Name}'.");
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

                // ===== BẮT ĐẦU GỬI EMAIL HÓA ĐƠN =====
                try
                {
                    var user = Appointment.User;
                    if (user != null)
                    {
                        var subject = $"Hóa đơn #{newInvoice.InvoiceId} cho xe {Appointment.Vehicle?.Model} đã sẵn sàng";
                        var message = $@"
                            <p>Chào {user.FullName},</p>
                            <p>Giai đoạn kiểm tra xe của bạn đã hoàn tất. Hóa đơn chi tiết cho dịch vụ và phụ tùng đã được tạo:</p>
                            <ul>
                                <li><strong>Mã hóa đơn:</strong> #HD-{newInvoice.InvoiceId}</li>
                                <li><strong>Tổng chi phí:</strong> {newInvoice.Amount:N0} đ</li>
                                <li><strong>Trạng thái:</strong> CHƯA THANH TOÁN</li>
                            </ul>
                            <p>Vui lòng đăng nhập vào tài khoản của bạn để xem chi tiết hóa đơn và tiến hành thanh toán.</p>
                            <p>Trân trọng,<br>Đội ngũ EV Service Center</p>";

                        await _emailSender.SendEmailAsync(user.Email, subject, message);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi gửi mail hóa đơn: {ex.Message}");
                    // Không dừng transaction nếu gửi mail lỗi
                }
                // ===== KẾT THÚC GỬI EMAIL =====

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["StatusMessage"] = $"Đã duyệt báo giá, trừ kho và tạo hóa đơn cho khách hàng.";

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError(string.Empty, $"Lỗi khi duyệt báo giá: {ex.Message}");
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