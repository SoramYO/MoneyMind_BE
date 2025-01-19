using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_DAL.Entities
{
    public class Transaction
    {
        public Transaction()
        {
            Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; } = null!;
        public DateTime TransactionDate { get; set; }
        public string RecipientName { get; set; }
        public Guid UserId { get; set; }
        public Guid CategoryId { get; set; }
        public virtual Category Category { get; set; } = null!;

    }
}
