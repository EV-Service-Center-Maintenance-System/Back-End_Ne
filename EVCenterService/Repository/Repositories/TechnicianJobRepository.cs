using EVCenterService.Data;
using EVCenterService.Models;
using EVCenterService.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EVCenterService.Repository.Repositories
{
    public class TechnicianJobRepository : ITechnicianJobRepository
    {
        private readonly EVServiceCenterContext _context;

        public TechnicianJobRepository(EVServiceCenterContext context) => _context = context;
        public async Task<OrderService?> GetJobByIdAsync(int orderId)
        {
            return await _context.OrderServices
                .Include(o => o.Vehicle)
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                    .ThenInclude(d => d.Service)
                    .Include(o => o.Slot)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task<List<OrderService>> GetJobsByTechnicianIdAsync(Guid technicianId)
        {
            return await _context.OrderServices
                .Include(o => o.Vehicle)
                .Include(o => o.OrderDetails)
                    .ThenInclude(d => d.Service)
                .Include(o => o.User)
                .Where(o => o.TechnicianId == technicianId)
                .OrderByDescending(o => o.AppointmentDate)
                .ToListAsync();
        }

        public async Task UpdateJobAsync(OrderService job)
        {
            _context.OrderServices.Update(job);
            await _context.SaveChangesAsync();
        }
    }
}
