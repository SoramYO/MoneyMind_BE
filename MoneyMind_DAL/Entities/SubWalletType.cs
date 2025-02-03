using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_DAL.Entities
{
    public class SubWalletType
    {
        public SubWalletType()
        {
            Id = Guid.NewGuid();
            CreateAt = DateTime.Now;
            IsActive = true;
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string IconPath { get; set; }  
        public string Color { get; set; } 
        public DateTime CreateAt { get; set; }
        public bool IsActive { get; set; }


        /// <summary>
        /// Properties related 
        /// </summary>
        public Guid UserId { get; set; }
        public Guid WalletTypeId { get; set; }
        public virtual WalletType WalletType { get; set; }
        public virtual ICollection<Wallet> Wallets { get; set; } = new List<Wallet>();
    }
}
