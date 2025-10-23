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

            appointment.Status = "InProgress";
            appointment.ChecklistNote += $"\n[Technician Assigned: {technicianId}]";
            await _repository.UpdateAsync(appointment);
        }

        public async Task<OrderService?> GetByIdAsync(int orderId)
        {
            return await _repository.GetByIdAsync(orderId);
        }
    }
}
