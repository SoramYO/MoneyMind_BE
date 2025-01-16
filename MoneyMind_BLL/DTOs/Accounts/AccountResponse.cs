using MoneyMind_BLL.DTOs.AccountTokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_BLL.DTOs.Accounts
{
    public class AccountResponse
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string[] Roles { get; set; }
        public AccountTokenResponse Tokens { get; set; }
    }
}
