﻿using EVCenterService.Data;
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
            return await _context.OrderServices
                .Include(o => o.Vehicle)
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task UpdateAsync(OrderService order)
        {
            _context.OrderServices.Update(order);
            await _context.SaveChangesAsync();
        }
    }
}
