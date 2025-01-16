using MoneyMind_BLL.DTOs.Emails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_BLL.Services.Interfaces
{
    public interface IMBBankSyncService
    {
        Task SyncTransactions(Guid userId);
    }
}