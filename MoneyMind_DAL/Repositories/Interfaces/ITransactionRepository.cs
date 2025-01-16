using MoneyMind_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_DAL.Repositories.Interfaces
{
    public interface ITransactionRepository : IGenericRepository<Transaction>
    {
        Task<Transaction> IsExistTransaction(string description, double amount);

        Task AddTransaction(Transaction transaction);
    }
}
