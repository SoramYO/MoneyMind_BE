using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_DAL.Entities
{
    public class WalletType
    {
        public WalletType()
        {
            Id = Guid.NewGuid();
            IsDisabled = false;
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDisabled { get; set; }

        /// <summary>
        /// Properties related 
        /// </summary>
        public virtual ICollection<WalletCategory> WalletCategories { get; set; } = new List<WalletCategory>();
        public virtual ICollection<GoalItem> GoalItems { get; set; } = new List<GoalItem>();
    }
}
