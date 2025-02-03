using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_DAL.Entities
{
    public class WalletType
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// Properties related 
        /// </summary>
        public virtual ICollection<SubWalletType> SubWalletTypes { get; set; } = new List<SubWalletType>();
        public virtual ICollection<GoalItem> GoalItems { get; set; } = new List<GoalItem>();
    }
}
