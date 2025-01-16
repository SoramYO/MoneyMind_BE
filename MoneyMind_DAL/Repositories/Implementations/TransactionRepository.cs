using Microsoft.EntityFrameworkCore;
using MoneyMind_DAL.DBContexts;
using MoneyMind_DAL.Entities;
using MoneyMind_DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_DAL.Repositories.Implementations
{
    public class TransactionRepository : GenericRepository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(MoneyMindDbContext context) : base(context)
        {
        }

        public async Task AddTransaction(Transaction transaction)
        {
            await _dbSet.AddAsync(transaction);
        }

        public async Task<Transaction> IsExistTransaction(string description, double amount)
        {
            var exist = await _dbSet.FirstOrDefaultAsync(t => t.Description.Equals(description) && t.Amount == amount);
            return exist;
        }
    }
}
