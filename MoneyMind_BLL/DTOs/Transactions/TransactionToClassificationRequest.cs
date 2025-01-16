using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_BLL.DTOs.Transactions
{
    public class TransactionToClassificationRequest
    {
        public string Description { get; set; }

        public float Amount { get; set; }
    }
}
