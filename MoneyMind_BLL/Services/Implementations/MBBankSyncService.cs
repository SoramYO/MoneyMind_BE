using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MoneyMind_BLL.DTOs.MBBank;

using MoneyMind_BLL.Services.Interfaces;
using MoneyMind_DAL.Entities;
using MoneyMind_DAL.Repositories.Interfaces;
namespace MoneyMind_BLL.Services.Implementations
{
    public class MBBankSyncService : IMBBankSyncService
    {
        private readonly HttpClient _httpClient;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMLService _mlService;
        private readonly IAccountBankRepository _accountBankRepository;

        public MBBankSyncService(
            HttpClient httpClient,
            ITransactionRepository transactionRepository,
            IMLService mlService,
            IAccountBankRepository accountBankRepository)
        {
            _httpClient = httpClient;
            _transactionRepository = transactionRepository;
            _mlService = mlService;
            _accountBankRepository = accountBankRepository;
        }

        public async Task SyncTransactions(Guid userId)
        {
            try
            {
                // Lấy thông tin tài khoản ngân hàng từ database
                var accountBank = await _accountBankRepository.GetByUserId(userId);
                if (accountBank == null)
                {
                    throw new InvalidOperationException("No bank account found for this user");
                }

                var request = new MBBankTransactionRequest
                {
                    Username = accountBank.Username,
                    Password = accountBank.Password,
                    AccountNumber = accountBank.AccountNumber,
                    Days = 1
                };

                var response = await _httpClient.PostAsJsonAsync("http://localhost:3000/api/mbbank/transactions", request);
                var result = await response.Content.ReadFromJsonAsync<MBBankTransactionResponse>();

                if (result?.Result?.Ok == true)
                {
                    foreach (var transaction in result.TransactionHistoryList)
                    {
                        if (decimal.TryParse(transaction.DebitAmount, out decimal debitAmount) && debitAmount > 0)
                        {
                            var existingTransaction = await _transactionRepository.IsExistTransaction(
                                transaction.Description,
                                (double)debitAmount
                            );

                            if (existingTransaction == null)
                            {
                                var category = await _mlService.ClassificationCategory(
                                    transaction.Description,
                                    (float)debitAmount
                                );

                                if (category != null)
                                {
                                    var newTransaction = new Transaction
                                    {
                                        Amount = (double)debitAmount,
                                        Description = transaction.Description,
                                        CategoryId = category.Id,
                                        TransactionDate = DateTime.ParseExact(transaction.TransactionDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                                        RecipientName = transaction.BenAccountName,
                                        Category = category,
                                        UserId = userId
                                    };

                                    await _transactionRepository.AddTransaction(newTransaction);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error syncing transactions: {ex.Message}");
                throw;
            }
        }
    }
}