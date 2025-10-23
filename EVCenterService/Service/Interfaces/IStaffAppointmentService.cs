using EVCenterService.Models;

namespace EVCenterService.Service.Interfaces
{
    public interface IStaffAppointmentService
    {
        Task<IEnumerable<OrderService>> GetPendingAppointmentsAsync();
        Task ConfirmAppointmentAsync(int orderId);
        Task RejectAppointmentAsync(int orderId);
        Task AssignTechnicianAsync(int orderId, Guid technicianId);
        Task<OrderService?> GetByIdAsync(int orderId);
        Task<OrderService> GetAppointmentWithDetailsAsync(int orderId);
    }
}
