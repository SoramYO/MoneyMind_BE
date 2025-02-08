using MoneyMind_BLL.DTOs.Tags;
using MoneyMind_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_BLL.DTOs.TransactionTags
{
    public class TransactionTagResponse
    {
        public Guid Id { get; set; }
        public Guid TransactionId { get; set; }
        public TagResponse Tag { get; set; }
    }
}
