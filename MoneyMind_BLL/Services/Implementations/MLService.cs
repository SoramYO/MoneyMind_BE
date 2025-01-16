using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using MoneyMind_BLL.DTOs.Transactions;
using MoneyMind_BLL.Services.Interfaces;
using MoneyMind_DAL.Entities;
using MoneyMind_DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoneyMind_BLL.Services.Implementations
{
    public class MLService : IMLService
    {
        private readonly PredictionEngine<TransactionClassification, TransactionPrediction> _predictionEngine;
        private readonly ICategoryRepository _categoryRepository;

        public MLService(MLContext mlContext, ITransformer loadedModel, ICategoryRepository categoryRepository)
        {
            _predictionEngine = mlContext.Model.CreatePredictionEngine<TransactionClassification, TransactionPrediction>(loadedModel);
            _categoryRepository = categoryRepository;
        }

        public async Task<Category> ClassificationCategory(string description, float amount)
        {
            var transaction = new TransactionClassification
            {
                Description = description,
                Amount = amount,
            };

            var prediction = _predictionEngine.Predict(transaction);
            Console.WriteLine($"Prediction: {prediction.PredictedCategory}");

            // Nếu dự đoán không hợp lệ, trả về null ngay lập tức
            if (prediction == null || prediction.PredictedCategory < 1 || prediction.PredictedCategory > 6)
                return null;

            // Truy vấn chỉ một lần cho tên category
            var categoryName = prediction.PredictedCategory switch
            {
                1 => "Necessities",
                2 => "Savings",
                3 => "Education",
                4 => "Leisure",
                5 => "Financial Freedom",
                6 => "Charity",
                _ => null
            };

            // Nếu categoryName là null (không hợp lệ), trả về null
            if (string.IsNullOrEmpty(categoryName))
                return null;

            // Truy vấn Category theo tên
            return await _categoryRepository.GetByName(categoryName);
        }
    }
}
