using EVCenterService.Data;
using EVCenterService.Models;
using EVCenterService.Repository.Interfaces; 
using EVCenterService.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using System; 
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; 

namespace EVCenterService.Service.Services
{
    public class StaffAppointmentService : IStaffAppointmentService
    {
        private readonly IStaffAppointmentRepository _appointmentRepository; 
        private readonly IAccountRepository _accountRepository;
        private readonly EVServiceCenterContext _context;
        private readonly IEmailSender _emailSender;

        public StaffAppointmentService(IStaffAppointmentRepository appointmentRepository, IAccountRepository accountRepository, EVServiceCenterContext context, IEmailSender emailSender)
        {
            _appointmentRepository = appointmentRepository;
            _accountRepository = accountRepository;
            _context = context;
            _emailSender = emailSender;
        }

        public async Task<IEnumerable<OrderService>> GetPendingAppointmentsAsync()
        {
            return await _appointmentRepository.GetPendingAppointmentsAsync();
        }

        public async Task<OrderService?> GetAppointmentWithDetailsAsync(int orderId) 
        {
            return await _appointmentRepository.GetByIdWithDetailsAsync(orderId);
        }

        public async Task ConfirmAppointmentAsync(int orderId)
        {
            var appointment = await _context.OrderServices
                                    .Include(o => o.User)
                                    .Include(o => o.OrderDetails)
                                        .ThenInclude(od => od.Service)
                                    .FirstOrDefaultAsync(o => o.OrderId == orderId)
                ?? throw new KeyNotFoundException("Không tìm thấy lịch hẹn.");

            appointment.Status = "Confirmed";

            _context.OrderServices.Update(appointment);

            try
            {
                if (appointment.User != null)
                {
                    var serviceNames = string.Join(", ", appointment.OrderDetails.Select(od => od.Service?.Name ?? "N/A"));
                    var subject = $"Lịch hẹn #{appointment.OrderId} của bạn đã được Xác nhận!";
                    var message = $@"
                        <p>Chào {appointment.User.FullName},</p>
                        <p>Chúng tôi vui mừng thông báo lịch hẹn của bạn đã được <strong>xác nhận</strong>:</p>
                        <ul>
                            <li><strong>Mã lịch hẹn:</strong> #{appointment.OrderId}</li>
                            <li><strong>Ngày giờ:</strong> {appointment.AppointmentDate:dd/MM/yyyy HH:mm}</li>
                            <li><strong>Dịch vụ:</strong> {serviceNames}</li>
                        </ul>
                        <p>Vui lòng đến trung tâm đúng hẹn. Chúng tôi rất mong được phục vụ bạn.</p>
                        <p>Trân trọng,<br>Đội ngũ EV Service Center</p>";

                    await _emailSender.SendEmailAsync(appointment.User.Email, subject, message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi gửi mail Xác nhận lịch: {ex.Message}");
            }

            await _context.SaveChangesAsync();
        }

        public async Task CancelAppointmentByStaffAsync(int orderId, string cancellationReason)
        {
            var appointment = await _context.OrderServices
                                        .Include(o => o.User) // Tải thông tin User để lấy email
                                        .FirstOrDefaultAsync(o => o.OrderId == orderId)
                ?? throw new KeyNotFoundException("Không tìm thấy lịch hẹn.");

            // Kiểm tra trạng thái cuối
            if (appointment.Status == "Completed" || appointment.Status == "Cancelled")
            {
                throw new InvalidOperationException("Không thể hủy lịch hẹn đã hoàn thành hoặc đã bị hủy.");
            }

            // 1. Cập nhật trạng thái
            appointment.Status = "Cancelled";

            // 2. Giải phóng Slot (nếu có)
            var slot = await _context.Slots.FirstOrDefaultAsync(s => s.OrderId == orderId);
            if (slot != null)
            {
                _context.Slots.Remove(slot); // Xóa slot để KTV rảnh
            }

            // 3. Gửi Email thông báo (Sử dụng EmailSender.cs)
            var customerEmail = appointment.User.Email;
            var subject = $"Thông báo Hủy Lịch hẹn #{orderId}";
            var message = $@"
                <p>Chào {appointment.User.FullName},</p>
                <p>Chúng tôi rất tiếc phải thông báo rằng lịch hẹn của bạn (Mã #{orderId}) vào lúc {appointment.AppointmentDate:dd/MM/yyyy HH:mm} đã bị hủy.</p>
                <p>Lý do: {cancellationReason}</p>
                <p>Chúng tôi thành thật xin lỗi vì sự bất tiện này. Vui lòng đặt lại lịch hẹn mới vào thời gian thuận tiện hơn.</p>
                <p>Trân trọng,<br>Đội ngũ EV Service Center</p>";

            // Không cần 'await' nếu bạn không cần biết kết quả
            await _emailSender.SendEmailAsync(customerEmail, subject, message);

            // 4. Lưu thay đổi
            await _context.SaveChangesAsync();
        }

        public async Task RejectAppointmentAsync(int orderId)
        {
            var appointment = await _context.OrderServices
                                    .Include(o => o.User)
                                    .FirstOrDefaultAsync(o => o.OrderId == orderId)
                ?? throw new KeyNotFoundException("Không tìm thấy lịch hẹn.");

            appointment.Status = "Cancelled";
            _context.OrderServices.Update(appointment);

            try
            {
                if (appointment.User != null)
                {
                    var subject = $"Thông báo về Lịch hẹn #{appointment.OrderId}";
                    var message = $@"
                        <p>Chào {appointment.User.FullName},</p>
                        <p>Chúng tôi rất tiếc phải thông báo rằng yêu cầu đặt lịch hẹn của bạn (Mã #{appointment.OrderId}) vào lúc {appointment.AppointmentDate:dd/MM/yyyy HH:mm} đã không thể được chấp nhận.</p>
                        <p><strong>Lý do:</strong> Hiện tại trung tâm không thể tiếp nhận thêm lịch hẹn vào khung giờ này.</p>
                        <p>Chúng tôi thành thật xin lỗi vì sự bất tiện này. Vui lòng đặt lại lịch hẹn mới vào thời gian khác thuận tiện hơn, hoặc liên hệ chúng tôi qua chat/hotline.</p>
                        <p>Trân trọng,<br>Đội ngũ EV Service Center</p>";

                    await _emailSender.SendEmailAsync(appointment.User.Email, subject, message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi gửi mail Từ chối lịch: {ex.Message}");
            }

            await _context.SaveChangesAsync();
        }

        public async Task AssignTechnicianAsync(int orderId, Guid technicianId)
        {
            // Lấy thông tin đơn hàng và chi tiết dịch vụ để tính thời gian kết thúc
            var appointment = await _context.OrderServices
                                        .Include(o => o.OrderDetails)
                                        .ThenInclude(od => od.Service)
                                        .FirstOrDefaultAsync(o => o.OrderId == orderId)
                ?? throw new KeyNotFoundException("Không tìm thấy lịch hẹn.");

            // Lấy thông tin kỹ thuật viên
            var technician = await _context.Accounts.FindAsync(technicianId);
            if (technician == null || technician.Role != "Technician")
                throw new KeyNotFoundException("Không tìm thấy kỹ thuật viên hợp lệ.");

            int assumedCenterId = 1;

            var slot = await _context.Slots.FirstOrDefaultAsync(s => s.OrderId == orderId);

            // Tính toán thời gian kết thúc dự kiến
            int totalDurationMinutes = appointment.OrderDetails.Sum(od => od.Service?.DurationMinutes ?? 0);
            DateTime startTime = appointment.AppointmentDate; // Lấy thời gian bắt đầu từ lịch hẹn
            DateTime endTime = startTime.AddMinutes(totalDurationMinutes);

            var technicianSlots = await _context.Slots
                .Include(s => s.Order)
                .Where(s => s.TechnicianId == technicianId &&
                            s.OrderId != orderId &&
                            s.Order != null &&
                            s.Order.Status != "Cancelled" &&
                            s.Order.Status != "Completed")
                .ToListAsync();

            bool isOverlapping = false;
            foreach (var existingSlot in technicianSlots)
            {
                if (startTime < existingSlot.EndTime && endTime > existingSlot.StartTime)
                {
                    isOverlapping = true;
                    break;
                }
            }

            if (isOverlapping)
            {
                throw new Exception($"Kỹ thuật viên này đã có lịch hẹn khác trùng vào khung giờ {startTime:HH:mm} - {endTime:HH:mm}.");
            }


            if (slot == null)
            {
                // Tạo Slot mới nếu chưa có
                slot = new Slot
                {
                    CenterId = assumedCenterId,
                    TechnicianId = technicianId,
                    OrderId = orderId,
                    StartTime = startTime,
                    EndTime = endTime
                };
                _context.Slots.Add(slot);
            }
            else
            {
                slot.CenterId = assumedCenterId; 
                slot.TechnicianId = technicianId;
                slot.StartTime = startTime; 
                slot.EndTime = endTime;
                _context.Slots.Update(slot);
            }

            // Cập nhật trạng thái OrderService
            appointment.Status = "InProgress"; 

            appointment.TechnicianId = technicianId;

            // Cập nhật ChecklistNote (Giữ nguyên logic cũ)
            var note = (appointment.ChecklistNote ?? "").Trim();
            if (note.Contains("Technician Assigned:"))
            {
                var index = note.IndexOf("Technician Assigned:");
                note = note[..index].TrimEnd();
            }
            appointment.ChecklistNote = string.IsNullOrWhiteSpace(note)
                ? $"Technician Assigned: {technician.FullName}"
                : $"{note}\nTechnician Assigned: {technician.FullName}";

            // Lưu tất cả thay đổi (OrderService và Slot)
            // await _repository.UpdateAsync(appointment); // Không cần dòng này nếu Repository dùng SaveChangesAsync trực tiếp từ Context
            await _context.SaveChangesAsync(); // Lưu cả Slot và OrderService cùng lúc
        }

        // Phương thức này có thể không cần thiết nữa nếu GetAppointmentWithDetailsAsync đủ dùng
        public async Task<OrderService?> GetByIdAsync(int orderId)
        {
            return await _appointmentRepository.GetByIdAsync(orderId);
        }
    }
}