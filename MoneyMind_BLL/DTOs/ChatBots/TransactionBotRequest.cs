using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_BLL.DTOs.ChatBots
{
    public class TransactionBotRequest
    {
        public string? RecipientName { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
    }
}
