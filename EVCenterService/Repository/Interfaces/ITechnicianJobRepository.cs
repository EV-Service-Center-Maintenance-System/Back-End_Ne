using EVCenterService.Models;

namespace EVCenterService.Repository.Interfaces
{
    public interface ITechnicianJobRepository
    {
        Task<List<OrderService>> GetJobsByTechnicianIdAsync(Guid technicianId);
        Task<OrderService?> GetJobByIdAsync(int orderId);
        Task UpdateJobAsync(OrderService job);
    }
}
