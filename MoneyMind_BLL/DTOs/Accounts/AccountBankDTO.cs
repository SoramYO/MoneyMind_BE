using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_BLL.DTOs.Accounts
{
    public class AccountBankDTO
    {
        public required string BankName { get; set; }
        public required string UserName { get; set; }
        public required string Password { get; set; }
        public required string AccountNumber { get; set; }
        public required string UserId { get; set; }
    }
}
