using MoneyMind_BLL.DTOs.WalletCategories;
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
        public string Name { get; set; }
        public string Description { get; set; }
        public double Balance { get; set; }
        public string Currency { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime LastUpdatedTime { get; set; }
        public Guid UserId { get; set; }
        public virtual WalletCategoryResponse WalletCategory { get; set; }
    }
}
