using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_BLL.DTOs.Wallets
{
    public class WalletRequest
    {
        [Range(0.01, double.MaxValue, ErrorMessage = "Balance must be greater than 0.")]
        public double Balance { get; set; }
        [Required(ErrorMessage = "WalletTypeId is required.")]
        public Guid WalletCategoryId { get; set; }
    }
}
