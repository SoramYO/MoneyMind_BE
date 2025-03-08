using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_DAL.Entities
{
    public class WalletCategory
    {
        public WalletCategory()
        {
            Id = Guid.NewGuid();
            CreateAt = DateTime.UtcNow; // Sử dụng UTC để tránh lỗi múi giờ
            IsActive = true;
        }
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty; // Tránh nullable string
        public string Description { get; set; } = string.Empty;
        public string IconPath { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public DateTime CreateAt { get; set; }
        public bool IsActive { get; set; }

        /// <summary>
        /// Properties related 
        /// </summary>
        public Guid UserId { get; set; }
        public Guid WalletTypeId { get; set; }
        public virtual WalletType WalletType { get; set; } = null!;
        public virtual ICollection<Wallet> Wallets { get; set; } = new List<Wallet>();
        public virtual ICollection<Activity> Activities { get; set; } = new List<Activity>();
    }

}
