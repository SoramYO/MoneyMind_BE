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
            CreateAt = DateTime.Now;
            IsActive = true;
        }
        public Guid Id { get; set; }
        public string? RecipientName { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; } = null!;
        public DateTime TransactionDate { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } 
        /// <summary>
        /// Properties related 
        /// </summary>
        public Guid UserId { get; set; }
        public Guid? WalletId { get; set; }
        public virtual Wallet? Wallet { get; set; }
        public Guid TagId { get; set; }
        public virtual Tag Tag { get; set; }
    }
}
