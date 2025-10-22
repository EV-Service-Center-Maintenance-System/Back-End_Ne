using EVCenterService.Data;
using EVCenterService.Models;
using EVCenterService.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EVCenterService.Repository.Repositories
{
    public class CustomerBookingRepository : ICustomerBookingRepository
    {
        private readonly EVServiceCenterContext _context;

        public CustomerBookingRepository(EVServiceCenterContext context) => _context = context;

        public async Task CreateAsync(OrderService order)
        {
            await _context.OrderServices.AddAsync(order);
        }

        public async Task DeleteAsync(OrderService order)
        {
            _context.OrderServices.Remove(order);
        }

        public async Task<IEnumerable<OrderService>> GetAllByUserAsync(Guid userId)
        {
            return await _context.OrderServices
                .Include(o => o.Vehicle)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.AppointmentDate)
                .ToListAsync();
        }

        public async Task<OrderService?> GetByIdAsync(int orderId, Guid userId)
        {
            return await _context.OrderServices
                .Include(o => o.Vehicle)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Service)
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(OrderService order)
        {
            _context.OrderServices.Update(order);
        }
    }
}
