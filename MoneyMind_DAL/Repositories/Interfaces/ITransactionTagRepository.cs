using MoneyMind_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_DAL.Repositories.Interfaces
{
    public interface ITransactionTagRepository : IGenericRepository<TransactionTag>
    {
        Task<bool> DeleteAllByTransactionId(Guid transactionId);
    }
}
