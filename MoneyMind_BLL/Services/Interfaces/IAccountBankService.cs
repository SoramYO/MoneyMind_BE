using MoneyMind_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_BLL.Services.Interfaces
{
    public interface IAccountBankService
    {
        Task<IEnumerable<AccountBank>> GetAccoutBankByUserIdAsync(Guid userId);
    }
}
