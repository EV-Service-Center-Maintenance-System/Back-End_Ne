using EVCenterService.Data;
using EVCenterService.Models;
using EVCenterService.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EVCenterService.Repository.Repositories
{
    public class AdminEmployeeRepository : IAdminEmployeeRepository
    {
        private readonly EVServiceCenterContext _context;

        public AdminEmployeeRepository(EVServiceCenterContext context)
        {
            _context = context;
        }

        public async Task<List<Account>> GetAllAsync()
        {
            return await _context.Accounts
                .Where(a => a.Role == "Staff" || a.Role == "Technician")
                .OrderBy(a => a.FullName)
                .ToListAsync();
        }

        public async Task<Account?> GetByIdAsync(Guid id)
        {
            return await _context.Accounts.FindAsync(id);
        }

        public async Task CreateAsync(Account employee)
        {
            _context.Accounts.Add(employee);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Account employee)
        {
            _context.Entry(employee).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task ToggleStatusAsync(Guid id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null) return;

            account.Status = account.Status == "Active" ? "Locked" : "Active";
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null) return;

            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();
        }
    }
}
