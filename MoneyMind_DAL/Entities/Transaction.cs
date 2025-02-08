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
            CreateAt = DateTime.UtcNow;
            IsActive = true;
        }
        public Guid Id { get; set; }
        public string? RecipientName { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdatedAt { get; set; } // Giữ null khi chưa cập nhật
        public bool IsActive { get; set; }

        /// <summary>
        /// Properties related 
        /// </summary>
        public Guid UserId { get; set; }
        public Guid? WalletId { get; set; }
        public virtual Wallet? Wallet { get; set; }
        public virtual ICollection<TransactionTag> TransactionTags { get; set; } = new List<TransactionTag>();

        // Quan hệ với Activity (Nhiều-Nhiều)
        public virtual ICollection<Activity> Activities { get; set; } = new List<Activity>();
    }

}
