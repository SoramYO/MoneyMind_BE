using Microsoft.ML;
using MoneyMind_BLL.MLModels;
using MoneyMind_BLL.DTOs.Transactions;
using MoneyMind_BLL.Services.Interfaces;
using MoneyMind_DAL.Entities;
using MoneyMind_DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MoneyMind_BLL.DTOs.Intents;

namespace MoneyMind_BLL.Services.Implementations
{
    public class ClassificationService : IClassificationService
    {
        private readonly PredictionEngine<TransactionData, TransactionPrediction> _transactionPredictionEngine;
        private readonly PredictionEngine<IntentData, IntentPrediction> _intentPredictionEngine;
        private readonly ITagRepository _tagRepository;

        public ClassificationService(IMLModel mlModel, ITagRepository tagRepository)
        {
            // Sử dụng các model đã được load trong MLModel singleton
            _transactionPredictionEngine = mlModel.MlContext.Model.CreatePredictionEngine<TransactionData, TransactionPrediction>(mlModel.TransactionModel);
            _intentPredictionEngine = mlModel.MlContext.Model.CreatePredictionEngine<IntentData, IntentPrediction>(mlModel.IntentModel);
            _tagRepository = tagRepository;
        }

        public async Task<Tag> ClassificationTag(string description)
        {
            var transaction = new TransactionData
            {
                Description = description
            };

            var prediction = _transactionPredictionEngine.Predict(transaction);
            Console.WriteLine($"Transaction Prediction: {prediction.PredictedCategory}");

            // Nếu dự đoán không hợp lệ, trả về null
            if (prediction == null || prediction.PredictedCategory < 1 || prediction.PredictedCategory > 35)
                return null;

            // Mapping từ ID sang tên tag
            var categoryMap = new Dictionary<int, string>
            {
                { 1, "Rent" }, { 2, "Utilities" }, { 3, "Groceries" }, { 4, "Transportation" },
                { 5, "Insurance" }, { 6, "Healthcare" }, { 7, "Investments" }, { 8, "Savings" },
                { 9, "Passive Income" }, { 10, "Debt Repayment" }, { 11, "Tuition" }, { 12, "Books" },
                { 13, "Online Courses" }, { 14, "Workshops" }, { 15, "School Supplies" }, { 16, "Dining Out" },
                { 17, "Travel" }, { 18, "Entertainment" }, { 19, "Shopping" }, { 20, "Hobbies" },
                { 21, "Fitness" }, { 22, "Donations" }, { 23, "Community Support" }, { 24, "Fundraising" },
                { 25, "Relief Aid" }, { 26, "Emergency Fund" }, { 27, "Retirement" }, { 28, "Future Purchases" },
                { 29, "Children's Fund" }, { 30, "Travel Savings" }, { 31, "Gifts" }, { 32, "Special Occasions" },
                { 33, "Subscriptions" }, { 34, "Uncategorized" }, { 35, "Fees & Charges" }
            };

            if (!categoryMap.TryGetValue(prediction.PredictedCategory, out var tagName))
                return null;

            // Truy vấn thông tin tag từ repository
            return await _tagRepository.GetByName(tagName);
        }

        public async Task<int> ClassificationIntent(string message)
        {
            var intentData = new IntentData
            {
                Message = message
            };

            var prediction = _intentPredictionEngine.Predict(intentData);
            Console.WriteLine($"Intent Prediction: {prediction.PredictedIntent}");

            return prediction.PredictedIntent;
        }
    }
}
