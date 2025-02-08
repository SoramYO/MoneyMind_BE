using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_DAL.Entities
{
    public class TransactionActivity
    {
        public Guid TransactionId { get; set; }
        public virtual Transaction Transaction { get; set; } = null!;

        public Guid ActivityId { get; set; }
        public virtual Activity Activity { get; set; } = null!;
    }

}
