using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_BLL.DTOs.ChatBots
{
    public class GoalItemBotRequest
    {
        public string Description { get; set; } = null!;
        public double UsedAmount { get; set; }
        public double UsedPercentage { get; set; }
        public string WalletTypeName { get; set; } = null!;
    }
}
