using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_BLL.DTOs.Transactions
{
    public class TransactionClassification
    {
        [LoadColumn(0)]
        public string Description { get; set; }

        [LoadColumn(1)]
        public float Amount { get; set; }

        [LoadColumn(2)]
        [ColumnName("Label")]
        public int Category { get; set; }

    }
}
