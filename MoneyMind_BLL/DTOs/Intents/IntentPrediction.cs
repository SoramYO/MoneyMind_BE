using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_BLL.DTOs.Intents
{
    public class IntentPrediction
    {
        [ColumnName("PredictedLabel")]
        public int PredictedIntent { get; set; }
    }
}
