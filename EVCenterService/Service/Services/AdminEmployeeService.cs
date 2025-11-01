using EVCenterService.Models;
using EVCenterService.Repository.Interfaces;
using EVCenterService.Service.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace EVCenterService.Service.Services
{
    public class AdminEmployeeService : IAdminEmployeeService
    {
        private readonly IAdminEmployeeRepository _repository;
        private readonly IPasswordHasher<Account> _passwordHasher;

        public AdminEmployeeService(IAdminEmployeeRepository repository, IPasswordHasher<Account> passwordHasher)
        {
            _repository = repository;
            _passwordHasher = passwordHasher;
        }

        public async Task<List<Account>> GetAllAsync() => await _repository.GetAllAsync();

        public async Task<Account?> GetByIdAsync(Guid id) => await _repository.GetByIdAsync(id);

        public async Task CreateAsync(Account employee)
        {
            employee.Password = _passwordHasher.HashPassword(employee, employee.Password);
            employee.Status = "Active";
            await _repository.CreateAsync(employee);
        }

        public async Task UpdateAsync(Account employee, string? newPassword = null)
        {
            if (!string.IsNullOrWhiteSpace(newPassword))
            {
                employee.Password = _passwordHasher.HashPassword(employee, newPassword);
            }
            await _repository.UpdateAsync(employee);
        }

        public async Task ToggleStatusAsync(Guid id) => await _repository.ToggleStatusAsync(id);

        public async Task DeleteAsync(Guid id) => await _repository.DeleteAsync(id);
    }
}
