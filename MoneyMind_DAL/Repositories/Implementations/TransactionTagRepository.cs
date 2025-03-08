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
    public class TransactionTagRepository : GenericRepository<TransactionTag>, ITransactionTagRepository
    {
        public TransactionTagRepository(MoneyMindDbContext context) : base(context)
        {
        }

        public async Task DeleteByTransactionIdAsync(Guid transactionId)
        {
            var transactionTags = await _context.TransactionTag
                                                .Where(tt => tt.TransactionId == transactionId)
                                                .ToListAsync();

            if (transactionTags.Any())
            {
                _context.TransactionTag.RemoveRange(transactionTags);
                await _context.SaveChangesAsync();
            }
        }

    }
}
