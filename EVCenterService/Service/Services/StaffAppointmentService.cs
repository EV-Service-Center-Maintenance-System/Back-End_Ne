using EVCenterService.Data;
using EVCenterService.Models;
using EVCenterService.Repository.Interfaces; 
using EVCenterService.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using System; 
using System.Collections.Generic; 
using System.Threading.Tasks; 

namespace EVCenterService.Service.Services
{
    public class StaffAppointmentService : IStaffAppointmentService
    {
        private readonly IStaffAppointmentRepository _appointmentRepository; 
        private readonly IAccountRepository _accountRepository;
        private readonly EVServiceCenterContext _context;

        public StaffAppointmentService(IStaffAppointmentRepository appointmentRepository, IAccountRepository accountRepository, EVServiceCenterContext context)
        {
            _appointmentRepository = appointmentRepository;
            _accountRepository = accountRepository;
            _context = context;
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
            // Dùng GetByIdAsync cơ bản vì chỉ cần cập nhật Status
            var appointment = await _appointmentRepository.GetByIdAsync(orderId)
                ?? throw new KeyNotFoundException("Không tìm thấy lịch hẹn.");

            appointment.Status = "Confirmed";
            await _appointmentRepository.UpdateAsync(appointment);
        }

        public async Task RejectAppointmentAsync(int orderId)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(orderId)
                ?? throw new KeyNotFoundException("Không tìm thấy lịch hẹn.");

            appointment.Status = "Cancelled";
            await _appointmentRepository.UpdateAsync(appointment);
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