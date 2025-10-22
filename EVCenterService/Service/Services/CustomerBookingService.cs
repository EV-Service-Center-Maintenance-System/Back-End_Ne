﻿using EVCenterService.Data;
using EVCenterService.Models;
using EVCenterService.Repository.Interfaces;
using EVCenterService.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EVCenterService.Service.Services
{
    public class CustomerBookingService : ICustomerBookingService
    {
        private readonly EVServiceCenterContext _context;

        public CustomerBookingService(EVServiceCenterContext context)
        {
            _context = context;
        }

        public async Task<OrderService> CreateBookingAsync(OrderService order, int serviceId = 0)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            order.Status ??= "Chờ xác nhận";
            order.TotalCost ??= 0;

            _context.OrderServices.Add(order);
            await _context.SaveChangesAsync();

            return order;
        }

        public async Task<IEnumerable<OrderService>> GetAllBookingsAsync(Guid userId)
        {
            return await _context.OrderServices
                .Include(o => o.OrderDetails)
                .ThenInclude(d => d.Service)
                .Include(o => o.Vehicle)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.AppointmentDate)
                .ToListAsync();
        }

        public async Task<OrderService?> GetBookingByIdAsync(int orderId, Guid userId)
        {
            return await _context.OrderServices
                .Include(o => o.OrderDetails)
                .ThenInclude(d => d.Service)
                .Include(o => o.Vehicle)
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);
        }

        public async Task UpdateBookingAsync(OrderService order, int serviceId = 0)
        {
            var existing = await _context.OrderServices
                .FirstOrDefaultAsync(o => o.OrderId == order.OrderId);

            if (existing == null)
                throw new Exception("Không tìm thấy lịch hẹn để cập nhật.");

            existing.AppointmentDate = order.AppointmentDate;
            existing.ChecklistNote = order.ChecklistNote;
            existing.Status = order.Status;

            _context.OrderServices.Update(existing);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBookingAsync(int orderId, Guid userId)
        {
            var booking = await _context.OrderServices
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);

            if (booking == null)
                throw new Exception("Không tìm thấy lịch hẹn.");

            _context.OrderServices.Remove(booking);
            await _context.SaveChangesAsync();
        }
    }
}
