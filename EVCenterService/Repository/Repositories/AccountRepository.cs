using EVCenterService.Data;
using EVCenterService.Models;
using EVCenterService.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace EVCenterService.Repository.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly EVServiceCenterContext _context;

        public AccountRepository(EVServiceCenterContext context)
        {
            _context = context;
        }

        public async Task<Account?> GetByIdAsync(Guid userId)
        {
            return await _context.Accounts.FindAsync(userId);
        }
    }
}