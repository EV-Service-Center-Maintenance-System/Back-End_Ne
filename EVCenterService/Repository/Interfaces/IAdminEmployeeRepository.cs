using EVCenterService.Models;

namespace EVCenterService.Repository.Interfaces
{
    public interface IAdminEmployeeRepository
    {
        Task<List<Account>> GetAllAsync();
        Task<Account?> GetByIdAsync(Guid id);
        Task CreateAsync(Account employee);
        Task UpdateAsync(Account employee);
        Task ToggleStatusAsync(Guid id);
        Task DeleteAsync(Guid id);
    }
}
