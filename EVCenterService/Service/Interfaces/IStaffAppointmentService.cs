using EVCenterService.Models;

namespace EVCenterService.Service.Interfaces
{
    public interface IStaffAppointmentService
    {
        Task<IEnumerable<OrderService>> GetPendingAppointmentsAsync();
        Task ConfirmAppointmentAsync(int orderId);
        Task RejectAppointmentAsync(int orderId);
        Task AssignTechnicianAsync(int orderId, Guid technicianId);
    }
}
