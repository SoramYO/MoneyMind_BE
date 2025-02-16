using MoneyMind_BLL.DTOs.Emails;
using MoneyMind_BLL.DTOs.GoogleSheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_BLL.Services.Interfaces
{
    public interface IGoogleSheetSyncService
    {
        Task<SyncResult> SyncTransactionsFromSheet(GoogleSheetRequest request);
    }

    public class SyncResult
    {
        public int NewTransactions { get; set; }
    }
}