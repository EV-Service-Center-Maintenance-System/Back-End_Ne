using EVCenterService.Models;

namespace EVCenterService.Service.Interfaces
{
    public interface IServiceCatalogService
    {
        Task<List<ServiceCatalog>> GetAllServicesAsync();
        Task<ServiceCatalog?> GetServiceByIdAsync(int id);
        Task CreateServiceAsync(ServiceCatalog service);
        Task UpdateServiceAsync(ServiceCatalog service);
        Task DeleteServiceAsync(int id);
    }
}
