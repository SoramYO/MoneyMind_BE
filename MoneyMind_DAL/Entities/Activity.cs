using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_DAL.Entities
{
    public class Activity
    {
        public Activity()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
        }

        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Properties related 
        /// </summary>
        public Guid SubWalletTypeId { get; set; }
        public virtual SubWalletType SubWalletType { get; set; } = null!;

        // Quan hệ Nhiều-Nhiều với Transaction
        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }

}
