using MoneyMind_BLL.DTOs.SubWalletTypes;
using MoneyMind_BLL.DTOs.Transactions;
using MoneyMind_BLL.DTOs.WalletTypes;
using MoneyMind_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_BLL.DTOs.Wallets
{
    public class WalletResponse
    {
        public Guid Id { get; set; }
        public double Balance { get; set; }
        public string Currency { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime LastUpdatedTime { get; set; }
        public Guid UserId { get; set; }
        public virtual SubWalletTypeResponse SubWalletType { get; set; }
    }
}
