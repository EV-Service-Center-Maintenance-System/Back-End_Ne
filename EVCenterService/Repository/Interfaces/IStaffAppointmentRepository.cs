using EVCenterService.Models;

namespace EVCenterService.Repository.Interfaces
{
    public interface IStaffAppointmentRepository
    {
        Task<IEnumerable<OrderService>> GetPendingAppointmentsAsync();
        Task<OrderService?> GetByIdAsync(int orderId);
        Task<OrderService?> GetByIdWithDetailsAsync(int orderId);
        Task UpdateAsync(OrderService order);
    }
}
