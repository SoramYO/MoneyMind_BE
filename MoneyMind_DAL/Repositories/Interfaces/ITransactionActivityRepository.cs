using MoneyMind_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_DAL.Repositories.Interfaces
{
    public interface ITransactionActivityRepository : IGenericRepository<TransactionActivity>
    {
        Task InsertRangeAsync(IEnumerable<TransactionActivity> transactionActivities);
        Task DeleteByTransactionIdAsync(Guid transactionId);
    }
}
