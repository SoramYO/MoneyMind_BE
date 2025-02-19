using Microsoft.Extensions.Configuration;
using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_BLL.MLModels
{
    public class MLModel : IMLModel
    {
        public MLContext MlContext { get; }
        public ITransformer TransactionModel { get; }
        public ITransformer IntentModel { get; }

        public MLModel(IConfiguration configuration)
        {
            // Khởi tạo MLContext
            MlContext = new MLContext();

            // Load transaction model
            var transactionModelPath = Path.Combine(AppContext.BaseDirectory, "transaction_model.zip");
            TransactionModel = MlContext.Model.Load(transactionModelPath, out _);

            // Load intent model
            var intentModelPath = Path.Combine(AppContext.BaseDirectory, "intent_model.zip");
            IntentModel = MlContext.Model.Load(intentModelPath, out _);
        }
    }
}
