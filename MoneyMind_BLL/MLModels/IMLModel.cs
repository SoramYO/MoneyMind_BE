using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_BLL.MLModels
{
    public interface IMLModel
    {
        MLContext MlContext { get; }
        ITransformer TransactionModel { get; }
        ITransformer IntentModel { get; }
    }
}
