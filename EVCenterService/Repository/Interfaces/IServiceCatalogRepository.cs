using EVCenterService.Models;

namespace EVCenterService.Repository.Interfaces
{
    public interface IServiceCatalogRepository
    {
        Task<List<ServiceCatalog>> GetAllAsync();
        Task<ServiceCatalog?> GetByIdAsync(int id);
        Task AddAsync(ServiceCatalog service);
        Task UpdateAsync(ServiceCatalog service);
        Task DeleteAsync(ServiceCatalog service);
        Task<bool> ExistsAsync(int id);
        Task<bool> IsServiceInUseAsync(int id);
    }
}
