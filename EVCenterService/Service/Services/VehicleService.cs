using EVCenterService.Models;
using EVCenterService.Repository.Interfaces;
using EVCenterService.Service.Interfaces;

namespace EVCenterService.Service.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _vehicleRepository;
        public VehicleService(IVehicleRepository vehicleRepository) => _vehicleRepository = vehicleRepository;
        public async Task AddVehicleAsync(Vehicle vehicle)
        {
            await _vehicleRepository.AddAsync(vehicle);
        }

        public async Task DeleteVehicleAsync(int id, Guid userId)
        {
            var vehicle = await _vehicleRepository.GetVehicleByIdAsync(id);
            if (vehicle == null || vehicle.UserId != userId)
                throw new UnauthorizedAccessException("You do not own this vehicle.");

            await _vehicleRepository.DeleteAsync(vehicle);
        }

        public async Task<Vehicle> GetVehicleByIdAsync(int id, Guid userId)
        {
            var vehicle = await _vehicleRepository.GetVehicleByIdAsync(id);
            if (vehicle == null || vehicle.UserId != userId)
                throw new UnauthorizedAccessException("You do not own this vehicle.");
            return vehicle;
        }

        public async Task<List<Vehicle>> GetVehiclesForCustomerAsync(Guid userId)
        {
            return await _vehicleRepository.GetVehiclesByUserIdAsync(userId);
        }

        public async Task UpdateVehicleAsync(Vehicle vehicle, Guid userId)
        {
            var existingVehicle = await _vehicleRepository.GetVehicleByIdAsync(vehicle.VehicleId);
            if (existingVehicle == null)
                throw new KeyNotFoundException("Vehicle not found.");

            if (existingVehicle.UserId != userId)
                throw new UnauthorizedAccessException("You do not have permission to modify this vehicle.");

            existingVehicle.Model = vehicle.Model;
            existingVehicle.Vin = vehicle.Vin;
            existingVehicle.BatteryCapacity = vehicle.BatteryCapacity;
            existingVehicle.Mileage = vehicle.Mileage;
            existingVehicle.LastMaintenanceDate = vehicle.LastMaintenanceDate;

            await _vehicleRepository.UpdateAsync(existingVehicle);
        }
        public async Task<bool> IsVinDuplicateAsync(string vin)
        {
            return await _vehicleRepository.VinExistsAsync(vin);
        }
    }
}
