using EVCenterService.Models;
using EVCenterService.ViewModels; 
using System; 
using System.Collections.Generic; 
using System.Threading.Tasks;

namespace EVCenterService.Service.Interfaces
{
    public interface ICustomerBookingService
    {
        Task<IEnumerable<OrderService>> GetAllBookingsAsync(Guid userId);
        Task<OrderService?> GetBookingByIdAsync(int orderId, Guid userId);
        Task<OrderService> CreateBookingAsync(OrderService order, int serviceId);
        Task UpdateBookingAsync(OrderService order, int serviceId);
        Task DeleteBookingAsync(int orderId, Guid userId);

        Task<List<AppointmentHistoryViewModel>> GetAppointmentHistoryAsync(Guid userId);
    }
}
