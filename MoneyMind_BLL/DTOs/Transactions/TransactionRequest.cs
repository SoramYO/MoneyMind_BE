using MoneyMind_DAL.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_BLL.DTOs.Transactions
{
    public class TransactionRequest
    {
        public string? RecipientName { get; set; } = null!;

        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public double Amount { get; set; }
        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; } = null!;

        [DataType(DataType.Date, ErrorMessage = "Transaction date must be a valid date.")]
        public DateTime TransactionDate { get; set; }
        public Guid? WalletId { get; set; }
    }
}
