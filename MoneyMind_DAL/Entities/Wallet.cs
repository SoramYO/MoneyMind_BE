using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_DAL.Entities
{
    public class Wallet
    {
        public Wallet()
        {
            Id = Guid.NewGuid();
            CreatedTime = DateTime.UtcNow;
            LastUpdatedTime = DateTime.UtcNow;
            Currency = "VND";
            IsActive = true;
        }
        public Guid Id { get; set; }
        public double Balance { get; set; }
        public string Currency { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime LastUpdatedTime { get; set; }
        public bool IsActive { get; set; }

        /// <summary>
        /// Properties related 
        /// </summary>
        public Guid UserId { get; set; }
        public Guid WalletCategoryId { get; set; }
        public virtual WalletCategory WalletCategory { get; set; } = null!;
        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }

}
