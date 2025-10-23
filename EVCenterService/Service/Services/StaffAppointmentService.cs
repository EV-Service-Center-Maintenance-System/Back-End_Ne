using EVCenterService.Models;
using EVCenterService.Repository.Interfaces;
using EVCenterService.Service.Interfaces;

namespace EVCenterService.Service.Services
{
    public class StaffAppointmentService : IStaffAppointmentService
    {
        private readonly IStaffAppointmentRepository _repository;

        public StaffAppointmentService(IStaffAppointmentRepository repository)
        {
            _repository = repository;
        }
        public async Task<IEnumerable<OrderService>> GetPendingAppointmentsAsync()
        {
            return await _repository.GetPendingAppointmentsAsync();
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
    }
}
