using Microsoft.EntityFrameworkCore;
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
    public class TransactionActivityRepository : GenericRepository<TransactionActivity>, ITransactionActivityRepository
    {
        public TransactionActivityRepository(MoneyMindDbContext context) : base(context)
        {
        }

        public async Task InsertRangeAsync(IEnumerable<TransactionActivity> transactionActivities)
        {
            if (transactionActivities == null || !transactionActivities.Any())
                return;

            await _context.TransactionActivity.AddRangeAsync(transactionActivities);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteByTransactionIdAsync(Guid transactionId)
        {
            var transactionActivity = await _context.TransactionActivity
                                                .Where(tt => tt.TransactionId == transactionId)
                                                .ToListAsync();

            if (transactionActivity.Any())
            {
                _context.TransactionActivity.RemoveRange(transactionActivity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
