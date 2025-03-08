using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_DAL.Entities
{
    public class TransactionTag
    {
        public TransactionTag()
        {
            CreatedAt = DateTime.UtcNow;
        }
        public Guid TransactionId { get; set; }
        public Guid TagId { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual Tag Tag { get; set; } = null!;
        public virtual Transaction Transaction { get; set; } = null!;
    }
}
