using EVCenterService.Data;
using EVCenterService.Models;
using EVCenterService.Repository.Interfaces;
using EVCenterService.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EVCenterService.Service.Services
{
    public class StaffAppointmentService : IStaffAppointmentService
    {
        private readonly IStaffAppointmentRepository _repository;
        private readonly EVServiceCenterContext _context;

        public StaffAppointmentService(IStaffAppointmentRepository repository, EVServiceCenterContext context)
        {
            _repository = repository;
            _context = context;
        }
        public async Task<IEnumerable<OrderService>> GetPendingAppointmentsAsync()
        {
            return await _repository.GetPendingAppointmentsAsync();
        }
        public async Task<OrderService> GetAppointmentWithDetailsAsync(int orderId)
        {
            return await _context.OrderServices
                .Include(o => o.User)
                .Include(o => o.Vehicle)
                .Include(o => o.OrderDetails)
                    .ThenInclude(d => d.Service)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task ConfirmAppointmentAsync(int orderId)
        {
            var appointment = await _repository.GetByIdAsync(orderId)
                ?? throw new Exception("Không tìm thấy lịch hẹn.");

            appointment.Status = "Confirmed";
            await _repository.UpdateAsync(appointment);
        }

        public async Task RejectAppointmentAsync(int orderId)
        {
            var appointment = await _repository.GetByIdAsync(orderId)
                ?? throw new Exception("Không tìm thấy lịch hẹn.");

            appointment.Status = "Cancelled";
            await _repository.UpdateAsync(appointment);
        }

        public async Task AssignTechnicianAsync(int orderId, Guid technicianId)
        {
            var appointment = await _repository.GetByIdAsync(orderId)
        ?? throw new Exception("Không tìm thấy lịch hẹn.");

            appointment.TechnicianId = technicianId;
            appointment.Status = "InProgress";

            var technician = await _context.Accounts
                .FirstOrDefaultAsync(a => a.UserId == technicianId);

            if (technician == null)
                throw new Exception("Không tìm thấy kỹ thuật viên.");

            var note = (appointment.ChecklistNote ?? "").Trim();

            if (note.Contains("Technician Assigned:"))
            {
                var index = note.IndexOf("Technician Assigned:");
                note = note[..index].TrimEnd();
            }

            appointment.ChecklistNote = string.IsNullOrWhiteSpace(note)
                ? $"Technician Assigned: {technician.FullName}"
                : $"{note}\nTechnician Assigned: {technician.FullName}";

            await _repository.UpdateAsync(appointment);
            await _context.SaveChangesAsync();
        }

        public async Task<OrderService?> GetByIdAsync(int orderId)
        {
            return await _repository.GetByIdAsync(orderId);
        }
    }
}
