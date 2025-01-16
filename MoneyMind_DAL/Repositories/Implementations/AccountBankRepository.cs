using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MoneyMind_DAL.DBContexts;
using MoneyMind_DAL.Entities;
using MoneyMind_DAL.Repositories.Interfaces;

namespace MoneyMind_DAL.Repositories.Implementations
{
    public class AccountBankRepository : GenericRepository<AccountBank>, IAccountBankRepository
    {

        public AccountBankRepository(MoneyMindDbContext context) : base(context)
        {

        }

        public async Task<AccountBank> GetByUserId(Guid userId)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public async Task<AccountBank> Add(AccountBank accountBank)
        {
            await _context.AccountBanks.AddAsync(accountBank);
            await _context.SaveChangesAsync();
            return accountBank;
        }

        public async Task<IEnumerable<AccountBank>> GetAllUserIds()
        {
            return await _context.AccountBanks.ToListAsync();
        }
    }
}