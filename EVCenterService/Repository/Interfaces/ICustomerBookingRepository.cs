using EVCenterService.Models;

namespace EVCenterService.Repository.Interfaces
{
    public interface ICustomerBookingRepository
    {
        Task<OrderService?> GetByIdAsync(int orderId, Guid userId);
        Task<IEnumerable<OrderService>> GetAllByUserAsync(Guid userId);
        Task CreateAsync(OrderService order);
        Task UpdateAsync(OrderService order);
        Task DeleteAsync(OrderService order);
        Task SaveChangesAsync();
    }
}
