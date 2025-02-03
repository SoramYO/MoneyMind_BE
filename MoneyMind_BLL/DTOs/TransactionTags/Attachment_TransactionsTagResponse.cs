using MoneyMind_BLL.DTOs.Tags;
using MoneyMind_BLL.DTOs.WalletTypes;
using MoneyMind_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_BLL.DTOs.TransactionTags
{
    public class Attachment_TransactionsTagResponse
    {
        public Guid Id { get; set; }
        public Guid TransactionId { get; set; }
        public Guid  TagId { get; set; }
    }
}
