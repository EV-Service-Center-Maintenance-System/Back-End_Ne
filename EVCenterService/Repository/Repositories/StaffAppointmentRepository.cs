using EVCenterService.Data;
using EVCenterService.Models;
using EVCenterService.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EVCenterService.Repository.Repositories
{
    public class StaffAppointmentRepository : IStaffAppointmentRepository
    {
        private readonly EVServiceCenterContext _context;
        public StaffAppointmentRepository(EVServiceCenterContext context) => _context = context;
        public async Task<IEnumerable<OrderService>> GetPendingAppointmentsAsync()
        {
            return await _context.OrderServices
                .Include(o => o.Vehicle)
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Service)
                .Where(o => o.Status == "Pending")
                .OrderBy(o => o.AppointmentDate)
                .ToListAsync();
        }

        public async Task<OrderService?> GetByIdAsync(int orderId)
        {
            return await _context.OrderServices.FindAsync(orderId);
        }

        public async Task<OrderService?> GetByIdWithDetailsAsync(int orderId)
        {
            // Lấy đầy đủ chi tiết cần thiết cho trang Details/Finalize
            return await _context.OrderServices
                .Include(o => o.User)
                .Include(o => o.Vehicle)
                .Include(o => o.OrderDetails)
                    .ThenInclude(d => d.Service)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task UpdateAsync(OrderService order)
        {
            // Update nên dùng Attach để hiệu quả hơn nếu đối tượng đã được theo dõi
            _context.Attach(order).State = EntityState.Modified;
            // _context.OrderServices.Update(order); // Hoặc dùng Update nếu chắc chắn nó chưa được theo dõi
            await _context.SaveChangesAsync();
        }
    }
}
