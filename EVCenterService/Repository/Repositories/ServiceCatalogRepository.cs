using EVCenterService.Data;
using EVCenterService.Models;
using EVCenterService.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EVCenterService.Repository.Repositories
{
    public class ServiceCatalogRepository : IServiceCatalogRepository
    {
        private readonly EVServiceCenterContext _context;

        public ServiceCatalogRepository(EVServiceCenterContext context) => _context = context;

        public async Task<List<ServiceCatalog>> GetAllAsync()
        {
            return await _context.ServiceCatalogs.ToListAsync();
        }

        public async Task<ServiceCatalog?> GetByIdAsync(int id)
        {
            return await _context.ServiceCatalogs.FirstOrDefaultAsync(s => s.ServiceId == id);
        }

        public async Task AddAsync(ServiceCatalog service)
        {
            _context.ServiceCatalogs.Add(service);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ServiceCatalog service)
        {
            _context.ServiceCatalogs.Update(service);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ServiceCatalog service)
        {
            _context.ServiceCatalogs.Remove(service);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.ServiceCatalogs.AnyAsync(s => s.ServiceId == id);
        }

        public async Task<bool> IsServiceInUseAsync(int id)
        {
            return await _context.OrderDetails.AnyAsync(od => od.ServiceId == id);
        }
    }
}
