using MoneyMind_BLL.Services.Interfaces;
using MoneyMind_DAL.Entities;
using MoneyMind_DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_BLL.Services.Implementations
{
    public class AccountBankService : IAccountBankService
    {
        private readonly IAccountBankRepository accountBankRepository;

        public AccountBankService(IAccountBankRepository accountBankRepository)
        {
            this.accountBankRepository = accountBankRepository;
        }

        public Task<AccountBank> AddAccountBankAsync(AccountBank accountBank)
        {
            return accountBankRepository.InsertAsync(accountBank);
        }

        public async Task<IEnumerable<AccountBank>> GetAccoutBankByUserIdAsync(Guid userId)
        {
            return await accountBankRepository.GetByUserId(userId);
        }
    }
}
