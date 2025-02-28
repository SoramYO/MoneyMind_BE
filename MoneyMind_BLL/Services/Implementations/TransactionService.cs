using AutoMapper;
using MoneyMind_BLL.DTOs;
using MoneyMind_BLL.DTOs.Activities;
using MoneyMind_BLL.DTOs.DataDefaults;
using MoneyMind_BLL.DTOs.MonthlyGoals;
using MoneyMind_BLL.DTOs.WalletCategories;
using MoneyMind_BLL.DTOs.Tags;
using MoneyMind_BLL.DTOs.Transactions;
using MoneyMind_BLL.DTOs.TransactionTags;
using MoneyMind_BLL.Services.Interfaces;
using MoneyMind_DAL.Entities;
using MoneyMind_DAL.Repositories.Implementations;
using MoneyMind_DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MoneyMind_BLL.Services.Implementations
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository transactionRepository;
        private readonly ITransactionTagRepository transactionTagRepository;
        private readonly ITransactionActivityRepository transactionActivityRepository;
        private readonly IMonthlyGoalRepository monthlyGoalRepository;
        private readonly IMonthlyGoalService monthlyGoalService;
        private readonly IGoalItemService goalItemService;
        private readonly IGoalItemRepository goalItemRepository;
        private readonly IWalletRepository walletRepository;
        private readonly IWalletService walletService;
        private readonly IWalletCategoryRepository walletCategoryRepository;
        private readonly IMapper mapper;
        private readonly IClassificationService classificationService;

        public TransactionService(ITransactionRepository transactionRepository,
            ITransactionTagRepository transactionTagRepository,
            ITransactionActivityRepository transactionActivityRepository,
            IMonthlyGoalRepository monthlyGoalRepository,
            IMonthlyGoalService monthlyGoalService,
            IGoalItemService goalItemService,
            IGoalItemRepository goalItemRepository,
            IWalletRepository walletRepository,
            IWalletService walletService,
            IWalletCategoryRepository walletCategoryRepository,
            IMapper mapper, IClassificationService classificationService)
        {
            this.transactionRepository = transactionRepository;
            this.transactionTagRepository = transactionTagRepository;
            this.transactionActivityRepository = transactionActivityRepository;
            this.monthlyGoalRepository = monthlyGoalRepository;
            this.monthlyGoalService = monthlyGoalService;
            this.goalItemService = goalItemService;
            this.goalItemRepository = goalItemRepository;
            this.walletRepository = walletRepository;
            this.walletService = walletService;
            this.walletCategoryRepository = walletCategoryRepository;
            this.mapper = mapper;
            this.classificationService = classificationService;
        }
        public async Task<TransactionResponse> AddTransactionAsync(Guid userId, TransactionRequest transactionRequest)
        {
            var jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "datadefaults.json");
            var jsonString = await File.ReadAllTextAsync(jsonFilePath);
            var dataDefaults = JsonSerializer.Deserialize<DataDefaults>(jsonString);

            // Tạo giao dịch mới
            var transactionDomain = mapper.Map<Transaction>(transactionRequest);
            transactionDomain.UserId = userId;
            transactionDomain = await transactionRepository.InsertAsync(transactionDomain);

            // Gán tag cho giao dịch
            var tag = await classificationService.ClassificationTag(transactionDomain.Description);
            await transactionTagRepository.InsertAsync(new TransactionTag { TransactionId = transactionDomain.Id, TagId = tag.Id });

            // Gán activities vào giao dịch
            if (transactionRequest.Activities.Any())
            {
                var transactionActivities = transactionRequest.Activities
                    .Select(activityId => new TransactionActivity
                    {
                        TransactionId = transactionDomain.Id,
                        ActivityId = activityId
                    }).ToList();

                await transactionActivityRepository.InsertRangeAsync(transactionActivities);
            }

            // Cập nhật mục tiêu hàng tháng nếu có
            var monthlyGoal = await monthlyGoalRepository.GetCurrentGoalForUserAsync(userId, transactionDomain.TransactionDate);
            if (monthlyGoal == null)
            {
                var monthlyGoalRequest = new MonthlyGoalRequest
                {
                    TotalAmount = dataDefaults.MonthlyGoal.TotalAmount,
                    Month = transactionDomain.TransactionDate.Month,
                    Year = transactionDomain.TransactionDate.Year
                };
                var monthlyGoalReponse = await monthlyGoalService.AddMonthlyGoalAsync(userId, monthlyGoalRequest);
                monthlyGoal = mapper.Map<MonthlyGoal>(monthlyGoalReponse);
            }
            if (transactionRequest.WalletId.HasValue)
            {
                // Trừ tiền trong ví
                await walletService.UpdateBalanceAsync(transactionRequest.WalletId.Value, -(double)transactionDomain.Amount);

                // Cập nhật GoalItem
                var wallet = await walletRepository.GetByIdAsync(transactionRequest.WalletId.Value, w => w.WalletCategory);
                await goalItemService.UpdateGoalItemAsync(userId, wallet.WalletCategory.WalletTypeId, monthlyGoal.Id,
                                                          (double)transactionDomain.Amount, (double)monthlyGoal.TotalAmount);
            }
            var response = mapper.Map<TransactionResponse>(transactionDomain);
            response.Tags = new List<TagResponse> { new TagResponse { Id = tag.Id, Name = tag.Name, Color = tag.Color, Description = tag.Description } };
            response.Activities = transactionRequest.Activities.Select(activityId => new ActivityResponse { Id = activityId }).ToList();

            return response;
        }


        public async Task<TransactionResponse> DeleteTransactionAsync(Guid transactionId, Guid userId)
        {
            var existingTransaction = await transactionRepository.GetByIdAsync(transactionId);
            if (existingTransaction == null || existingTransaction.UserId != userId)
            {
                return null;
            }

            var monthlyGoal = await monthlyGoalRepository.GetCurrentGoalForUserAsync(userId, existingTransaction.TransactionDate);

            if (monthlyGoal != null && existingTransaction.WalletId.HasValue)
            {
                // Cộng lại số tiền cũ vào ví
                await walletService.UpdateBalanceAsync(
                    existingTransaction.WalletId.Value,
                    (double)existingTransaction.Amount
                );

                // Trừ (revert) usedAmount trong goalItem
                var wallet = await walletRepository.GetByIdAsync(existingTransaction.WalletId.Value, w => w.WalletCategory);
                var walletCategory = await walletCategoryRepository.GetByIdAsync(wallet.WalletCategory.Id);

                await goalItemService.UpdateGoalItemAsync(
                    userId,
                    walletCategory.WalletTypeId,
                    monthlyGoal.Id,
                    -(double)existingTransaction.Amount,
                    (double)monthlyGoal.TotalAmount
                );

                // Cập nhật trạng thái tổng thể của MonthlyGoal
            }

            // Xóa mềm giao dịch
            existingTransaction.IsActive = false;
            existingTransaction = await transactionRepository.UpdateAsync(existingTransaction);

            return mapper.Map<TransactionResponse>(existingTransaction);
        }


        public async Task<ListDataResponse> GetTransactionAsync(Expression<Func<Transaction, bool>>? filter, Func<IQueryable<Transaction>, IOrderedQueryable<Transaction>> orderBy, string includeProperties, int pageIndex, int pageSize)
        {
            var response = await transactionRepository.GetAsync(
                        filter: filter,
                        orderBy: orderBy,
                        includeProperties: includeProperties,
                        pageIndex: pageIndex,
                        pageSize: pageSize
                        );
            var transactions = response.Item1.ToList(); // Convert to List to allow indexing
            var totalPages = response.Item2;
            var totalRecords = response.Item3;

            var transactionResponse = mapper.Map<List<TransactionResponse>>(transactions);

            for (int i = 0; i < transactions.Count; i++)
            {
                transactionResponse[i].Tags = transactions[i].TransactionTags
                    .Select(tt => new TagResponse
                    {
                        Id = tt.Tag.Id,
                        Name = tt.Tag.Name,
                        Color = tt.Tag.Color,
                        Description = tt.Tag.Description
                    }).ToList();

                transactionResponse[i].Activities = transactions[i].TransactionActivities
                    .Select(ta => new ActivityResponse
                    {
                        Id = ta.Activity.Id,
                        Name = ta.Activity.Name,
                        Description = ta.Activity.Description
                    }).ToList();
            }

            var listResponse = new ListDataResponse
            {
                TotalRecord = totalRecords,
                TotalPage = totalPages,
                PageIndex = pageIndex,
                Data = transactionResponse
            };

            return listResponse;
        }

        public async Task<TransactionResponse> GetTransactionByIdAsync(Guid transactionId)
        {
            var transaction = await transactionRepository.GetByIdAsync(
                transactionId,
                t => t.TransactionTags // Bao gồm bảng trung gian TransactionTag
            );

            if (transaction == null)
            {
                return null;
            }

            // Lấy danh sách Tags từ bảng TransactionTags
            var transactionResponse = mapper.Map<TransactionResponse>(transaction);
            transactionResponse.Tags = transaction.TransactionTags.
               Where(tt => tt.Tag != null)
              .Select(tt => new TagResponse
              {
                  Id = tt.Tag.Id,
                  Name = tt.Tag.Name,
                  Color = tt.Tag.Color,
                  Description = tt.Tag.Description
              }).ToList();

            return transactionResponse;
        }


        public async Task<TransactionResponse> UpdateTransactionAsync(Guid transactionId, Guid userId, TransactionRequest transactionRequest)
        {
            var jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "datadefaults.json");
            var jsonString = await File.ReadAllTextAsync(jsonFilePath);
            var dataDefaults = JsonSerializer.Deserialize<DataDefaults>(jsonString);

            var existingTransaction = await transactionRepository.GetByIdAsync(transactionId, t => t.Wallet.WalletCategory);
            if (existingTransaction == null || existingTransaction.UserId != userId)
            {
                return null;
            }

            var amountDifference = transactionRequest.Amount - existingTransaction.Amount;
            var monthlyGoal = await monthlyGoalRepository.GetCurrentGoalForUserAsync(userId, existingTransaction.TransactionDate);
            if (monthlyGoal == null)
            {
                var monthlyGoalRequest = new MonthlyGoalRequest
                {
                    TotalAmount = dataDefaults.MonthlyGoal.TotalAmount,
                    Month = existingTransaction.TransactionDate.Month,
                    Year = existingTransaction.TransactionDate.Year
                };
                var monthlyGoalReponse = await monthlyGoalService.AddMonthlyGoalAsync(userId, monthlyGoalRequest);
                monthlyGoal = mapper.Map<MonthlyGoal>(monthlyGoalReponse);
            }

            // Kiểm tra thay đổi WalletId
            bool isWalletChanged = existingTransaction.WalletId != transactionRequest.WalletId;
            bool isNewWalletAdded = existingTransaction.WalletId == null && transactionRequest.WalletId.HasValue;

            if (monthlyGoal != null)
            {
                if (isNewWalletAdded || isWalletChanged)
                {
                    // Nếu có Wallet cũ, trừ số tiền đã sử dụng và cập nhật số dư
                    if (existingTransaction.WalletId.HasValue)
                    {
                        // B1: Cộng lại tiền cũ vào ví
                        await walletService.UpdateBalanceAsync(
                            existingTransaction.WalletId.Value,
                            (double)existingTransaction.Amount // revert lại số tiền cũ
                        );

                        // B2: Giảm UsedAmount trong GoalItem cũ
                        var oldWallet = await walletRepository.GetByIdAsync(existingTransaction.WalletId.Value);
                        var oldSubWalletType = await walletCategoryRepository.GetByIdAsync(oldWallet.WalletCategory.Id);

                        await goalItemService.UpdateGoalItemAsync(
                            userId,
                            oldSubWalletType.WalletTypeId,
                            monthlyGoal.Id,
                            -(double)existingTransaction.Amount,
                            (double)monthlyGoal.TotalAmount
                        );
                    }

                    // Nếu có Wallet mới, trừ số tiền khỏi Wallet mới
                    if (transactionRequest.WalletId.HasValue)
                    {
                        // B1: Trừ tiền khỏi ví mới
                        await walletService.UpdateBalanceAsync(
                            transactionRequest.WalletId.Value,
                            -(double)transactionRequest.Amount
                        );

                        // B2: Tăng UsedAmount trong GoalItem mới
                        var newWallet = await walletRepository.GetByIdAsync(transactionRequest.WalletId.Value, w => w.WalletCategory);
                        var newWalletCategory = await walletCategoryRepository.GetByIdAsync(newWallet.WalletCategory.Id);

                        await goalItemService.UpdateGoalItemAsync(
                            userId,
                            newWalletCategory.WalletTypeId,
                            monthlyGoal.Id,
                            (double)transactionRequest.Amount,
                            (double)monthlyGoal.TotalAmount
                        );
                    }
                }
                else if (transactionRequest.WalletId.HasValue) // Nếu không đổi Wallet nhưng cập nhật số tiền
                {
                    // B1: Cập nhật lại ví theo chênh lệch
                    await walletService.UpdateBalanceAsync(
                        transactionRequest.WalletId.Value,
                        -(double)amountDifference
                    );

                    // B2: Cập nhật GoalItem
                    var wallet = await walletRepository.GetByIdAsync(transactionRequest.WalletId.Value);
                    var walletCategory = await walletCategoryRepository.GetByIdAsync(wallet.WalletCategory.Id);

                    await goalItemService.UpdateGoalItemAsync(
                        userId,
                        walletCategory.WalletTypeId,
                        monthlyGoal.Id,
                        (double)amountDifference,
                        (double)monthlyGoal.TotalAmount
                    );
                }
            }

            // Cập nhật thông tin giao dịch
            existingTransaction.RecipientName = transactionRequest.RecipientName;
            existingTransaction.Amount = transactionRequest.Amount;
            existingTransaction.Description = transactionRequest.Description;
            existingTransaction.TransactionDate = transactionRequest.TransactionDate;
            existingTransaction.WalletId = transactionRequest.WalletId;
            existingTransaction.UpdatedAt = DateTime.UtcNow;

            // **Xóa các TransactionTag cũ trước khi cập nhật**
            await transactionTagRepository.DeleteByTransactionIdAsync(existingTransaction.Id);

            // **Gán Tag mới bằng AI**
            var tag = await classificationService.ClassificationTag(existingTransaction.Description);
            await transactionTagRepository.InsertAsync(new TransactionTag
            {
                TransactionId = existingTransaction.Id,
                TagId = tag.Id
            });

            await transactionActivityRepository.DeleteByTransactionIdAsync(existingTransaction.Id);

            // **Thêm danh sách TransactionActivities mới**
            if (transactionRequest.Activities.Any())
            {
                var transactionActivities = transactionRequest.Activities
                    .Select(activityId => new TransactionActivity
                    {
                        TransactionId = existingTransaction.Id,
                        ActivityId = activityId
                    }).ToList();

                await transactionActivityRepository.InsertRangeAsync(transactionActivities);
            }

            existingTransaction = await transactionRepository.UpdateAsync(existingTransaction);

            return mapper.Map<TransactionResponse>(existingTransaction);
        }

    }
}
