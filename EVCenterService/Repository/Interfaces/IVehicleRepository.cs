using EVCenterService.Models;

namespace EVCenterService.Repository.Interfaces
{
    public interface IVehicleRepository
    {
        Task<List<Vehicle>> GetVehiclesByUserIdAsync(Guid userId);
        Task<Vehicle> GetVehicleByIdAsync(int id);
        Task AddAsync(Vehicle vehicle);
        Task UpdateAsync(Vehicle vehicle);
        Task DeleteAsync(Vehicle vehicle);
        Task<bool> IsVehicleOwnedByUserAsync(int id, Guid userId);
        Task<bool> VinExistsAsync(string vin);
    }
}
