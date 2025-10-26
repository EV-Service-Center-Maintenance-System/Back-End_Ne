using EVCenterService.Models;

namespace EVCenterService.Service.Interfaces
{
    public interface ITechnicianJobService
    {
        Task<List<OrderService>> GetAssignedJobsAsync(Guid technicianId);
        Task<OrderService?> GetJobDetailAsync(int orderId);
        Task CompleteJobAsync(int orderId, string? note);
        Task CancelJobAsync(int orderId, string? note);
    }
}
