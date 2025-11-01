using EVCenterService.Models;
using EVCenterService.Repository.Interfaces;
using EVCenterService.Service.Interfaces;

namespace EVCenterService.Service.Services
{
    public class ServiceCatalogService : IServiceCatalogService
    {
        private readonly IServiceCatalogRepository _repository;

        public ServiceCatalogService(IServiceCatalogRepository repository) => _repository = repository;
        public async Task<List<ServiceCatalog>> GetAllServicesAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<ServiceCatalog?> GetServiceByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task CreateServiceAsync(ServiceCatalog service)
        {
            await _repository.AddAsync(service);
        }

        public async Task UpdateServiceAsync(ServiceCatalog service)
        {
            if (!await _repository.ExistsAsync(service.ServiceId))
                throw new KeyNotFoundException($"Không tìm thấy dịch vụ ID: {service.ServiceId}");

            await _repository.UpdateAsync(service);
        }

        public async Task DeleteServiceAsync(int id)
        {
            var service = await _repository.GetByIdAsync(id);
            if (service == null)
                throw new KeyNotFoundException("Không tìm thấy dịch vụ để xóa.");

            var inUse = await _repository.IsServiceInUseAsync(id);
            if (inUse)
                throw new InvalidOperationException("Dịch vụ đang được sử dụng trong đơn hàng, không thể xóa.");

            await _repository.DeleteAsync(service);
        }
    }
}
