using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_BLL.DTOs.Transactions
{
    public class TransactionPrediction
    {
        [ColumnName("PredictedLabel")]
        public int PredictedCategory { get; set; }
    }
}
