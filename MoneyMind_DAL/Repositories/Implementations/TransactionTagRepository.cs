using MoneyMind_DAL.DBContexts;
using MoneyMind_DAL.Entities;
using MoneyMind_DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_DAL.Repositories.Implementations
{
    public class TransactionTagRepository : GenericRepository<TransactionTag>, ITransactionTagRepository
    {
        public TransactionTagRepository(MoneyMindDbContext context) : base(context)
        {
        }

        public Task<bool> DeleteAllByTransactionId(Guid transactionId)
        {
            return Task.Run(() =>
            {
                var transactionTags = _dbSet.Where(tt => tt.TransactionId == transactionId).ToList();
                _dbSet.RemoveRange(transactionTags);
                return true;
            });
        }
    }
}
