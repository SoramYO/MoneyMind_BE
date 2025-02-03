using MoneyMind_BLL.DTOs.TransactionTags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_BLL.DTOs.Transactions
{
    public class Attachment_TransactionResponse
    {
        public Guid Id { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; } = null!;
        public DateTime TransactionDate { get; set; }
        public DateTime CreateAt { get; set; }
        public virtual ICollection<Attachment_TransactionsTagResponse> TransactionTags { get; set; } = new List<Attachment_TransactionsTagResponse>();
    }
}
