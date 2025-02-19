using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_BLL.DTOs.ChatBots
{
    public class WalletBotRequest
    {
        public double Balance { get; set; }
        public string Currency { get; set; } = null!;
        public string WalletCategoryName { get; set; } = null!;
    }
}
