using MoneyMind_BLL.DTOs.WalletCategories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_BLL.DTOs.Activities
{
    public class ActivityRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid WalletCategoryId { get; set; }
    }
}
