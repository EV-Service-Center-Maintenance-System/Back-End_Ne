using EVCenterService.Models;

namespace EVCenterService.Service.Interfaces
{
    public interface IAdminEmployeeService
    {
        Task<List<Account>> GetAllAsync();
        Task<Account?> GetByIdAsync(Guid id);
        Task CreateAsync(Account employee);
        Task UpdateAsync(Account employee, string? newPassword = null);
        Task ToggleStatusAsync(Guid id);
        Task DeleteAsync(Guid id);
    }
}
