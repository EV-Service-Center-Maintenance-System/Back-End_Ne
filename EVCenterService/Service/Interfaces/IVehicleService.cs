using EVCenterService.Models;

namespace EVCenterService.Service.Interfaces
{
    public interface IVehicleService
    {
        Task<List<Vehicle>> GetVehiclesForCustomerAsync(Guid userId);
        Task<Vehicle> GetVehicleByIdAsync(int id, Guid userId);
        Task AddVehicleAsync(Vehicle vehicle);
        Task UpdateVehicleAsync(Vehicle vehicle, Guid userId);
        Task DeleteVehicleAsync(int id, Guid userId);
    }
}
