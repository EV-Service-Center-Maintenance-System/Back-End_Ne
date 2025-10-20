using EVCenterService.Data;
using EVCenterService.Models;
using EVCenterService.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EVCenterService.Repository.Repositories
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly EVServiceCenterContext _context;
        public VehicleRepository(EVServiceCenterContext context) => _context = context;

        public async Task AddAsync(Vehicle vehicle)
        {
            await _context.Vehicles.AddAsync(vehicle);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Vehicle vehicle)
        {
            _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();
        }

        public async Task<Vehicle> GetVehicleByIdAsync(int id)
        {
            return await _context.Vehicles.FindAsync(id);
        }

        public async Task<List<Vehicle>> GetVehiclesByUserIdAsync(Guid userId)
        {
            return await _context.Vehicles
                .Where(v => v.UserId == userId)
                .ToListAsync();
        }

        public async Task<bool> IsVehicleOwnedByUserAsync(int id, Guid userId)
        {
            return await _context.Vehicles.AnyAsync(v => v.VehicleId == id && v.UserId == userId);
        }

        public async Task UpdateAsync(Vehicle vehicle)
        {
            _context.Vehicles.Update(vehicle);
            await _context.SaveChangesAsync();
        }
    }
}
