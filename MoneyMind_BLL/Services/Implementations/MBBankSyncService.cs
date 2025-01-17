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
        private readonly ITransactionSyncLogRepository _transactionSyncLogRepository;

        public MBBankSyncService(
            HttpClient httpClient,
            ITransactionRepository transactionRepository,
            IMLService mlService,
            IAccountBankRepository accountBankRepository,
            ITransactionSyncLogRepository transactionSyncLogRepository)
        {
            _httpClient = httpClient;
            _transactionRepository = transactionRepository;
            _mlService = mlService;
            _accountBankRepository = accountBankRepository;
            _transactionSyncLogRepository = transactionSyncLogRepository;
        }

        public async Task SyncTransactions(Guid userId)
        {
            TransactionSyncLog syncLog = new TransactionSyncLog
            {
                UserId = userId
            };

            try
            {
                // Lưu trạng thái ban đầu của log vào cơ sở dữ liệu
                await _transactionSyncLogRepository.InsertAsync(syncLog);

                // Lấy thông tin tài khoản ngân hàng từ database
                var accountBanks = await _accountBankRepository.GetByUserId(userId);
                if (accountBanks == null || !accountBanks.Any())
                {
                    throw new InvalidOperationException("No bank accounts found for this user.");
                }

                foreach (var accountBank in accountBanks)
                {
                    if (accountBank == null)
                    {
                        continue; // Bỏ qua tài khoản null
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

                                        await _transactionRepository.InsertAsync(newTransaction);
                                    }
                                }
                            }
                        }
                    }
                }

                // Cập nhật trạng thái log khi thành công
                syncLog.Status = "Success";
                await _transactionSyncLogRepository.UpdateAsync(syncLog);
            }
            catch (Exception ex)
            {
                // Ghi lại lỗi trong log
                syncLog.Status = "Failed";
                syncLog.ErrorMessage = ex.Message;
                await _transactionSyncLogRepository.UpdateAsync(syncLog);

                // Ghi lỗi ra console và throw lại exception
                Console.WriteLine($"Error syncing transactions: {ex.Message}");
                throw;
            }
        }
    }
}