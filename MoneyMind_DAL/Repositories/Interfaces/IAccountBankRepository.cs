using MoneyMind_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_DAL.Repositories.Interfaces
{
    public interface IAccountBankRepository : IGenericRepository<AccountBank>
    {
        Task<IEnumerable<AccountBank>> GetByUserId(Guid userId);

        //Task<IEnumerable<AccountBank>> GetAllUserIds();
        //Task<AccountBank> Add(AccountBank accountBank);
    }
}