using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_BLL.DTOs.Accounts
{
    public class AccountLoginGoogleRequest
    {
        [Required]
        public string Token { get; set; }
    }
}
