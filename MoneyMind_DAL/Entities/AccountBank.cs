using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_DAL.Entities
{
    public class AccountBank
    {
        public AccountBank()
        {
            Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string AccountNumber { get; set; }
        public Guid UserId { get; set; }
    }
}