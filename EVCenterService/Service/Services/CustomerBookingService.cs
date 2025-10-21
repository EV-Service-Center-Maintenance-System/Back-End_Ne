using EVCenterService.Data;
using EVCenterService.Models;
using EVCenterService.Repository.Interfaces;
using EVCenterService.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EVCenterService.Service.Services
{
    public class CustomerBookingService : ICustomerBookingService
    {
        private readonly ICustomerBookingRepository _repo;
        private readonly EVServiceCenterContext _context;

        public CustomerBookingService(ICustomerBookingRepository repo, EVServiceCenterContext context)
        {
            _repo = repo;
            _context = context;
        }

        public async Task<IEnumerable<OrderService>> GetAllBookingsAsync(Guid userId)
        {
            return await _repo.GetAllByUserAsync(userId);
        }

        public async Task<OrderService?> GetBookingByIdAsync(int orderId, Guid userId)
        {
            return await _repo.GetByIdAsync(orderId, userId);
        }

        public async Task<OrderService> CreateBookingAsync(OrderService order, int serviceId)
        {
            var service = await _context.ServiceCatalogs.FirstOrDefaultAsync(s => s.ServiceId == serviceId);
            if (service == null)
                throw new Exception("Dịch vụ không tồn tại.");

            order.Status = "Pending";
            order.TotalCost = service.BasePrice ?? 0;
            order.AppointmentDate = order.AppointmentDate.ToLocalTime();

            await _repo.CreateAsync(order);
            await _repo.SaveChangesAsync();

            var detail = new OrderDetail
            {
                OrderId = order.OrderId,
                ServiceId = serviceId,
                Quantity = 1,
                UnitPrice = service.BasePrice
            };

            _context.OrderDetails.Add(detail);
            await _context.SaveChangesAsync();

            return order;
        }

        public async Task UpdateBookingAsync(OrderService order, int serviceId)
        {
            var existing = await _repo.GetByIdAsync(order.OrderId, order.UserId ?? Guid.Empty);
            if (existing == null)
                throw new Exception("Không tìm thấy lịch hẹn.");

            var service = await _context.ServiceCatalogs.FirstOrDefaultAsync(s => s.ServiceId == serviceId);
            if (service == null)
                throw new Exception("Dịch vụ không tồn tại.");

            existing.VehicleId = order.VehicleId;
            existing.AppointmentDate = order.AppointmentDate.ToLocalTime();
            existing.ChecklistNote = order.ChecklistNote;
            existing.TotalCost = service.BasePrice ?? 0;

            await _repo.UpdateAsync(existing);
            await _repo.SaveChangesAsync();

            var detail = await _context.OrderDetails.FirstOrDefaultAsync(d => d.OrderId == existing.OrderId);
            if (detail != null)
            {
                detail.ServiceId = serviceId;
                detail.UnitPrice = service.BasePrice;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteBookingAsync(int orderId, Guid userId)
        {
            var existing = await _repo.GetByIdAsync(orderId, userId);
            if (existing == null)
                throw new Exception("Không tìm thấy lịch hẹn.");

            await _repo.DeleteAsync(existing);
            await _repo.SaveChangesAsync();
        }
    }
}
