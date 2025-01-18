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

        public async Task<AccountBank> Add(AccountBank accountBank)
        {
            await _dbSet.AddAsync(accountBank);
            await _context.SaveChangesAsync();
            return accountBank;
        }

        public async Task<IEnumerable<AccountBank>> GetByUserId(Guid userId)
        {
            return await _dbSet.Where(a => a.UserId == userId).ToListAsync();
        }

    }
}