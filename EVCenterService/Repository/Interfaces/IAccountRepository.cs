using EVCenterService.Models;
using System;
using System.Threading.Tasks;

namespace EVCenterService.Repository.Interfaces
{
    public interface IAccountRepository
    {
        Task<Account?> GetByIdAsync(Guid userId);
    }
}