using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_DAL.Entities
{
    public class TransactionSyncLog
    {
        public TransactionSyncLog()
        {
            Id = Guid.NewGuid();
            SyncTime = DateTime.Now;
            Status = "InProcess";
        }
        public Guid Id { get; set; }
        public DateTime SyncTime { get; set; }
        public string Status { get; set; }
        public string ErrorMessage { get; set; }
        public Guid UserId { get; set; }
    }
}
