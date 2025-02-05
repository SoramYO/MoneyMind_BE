using AutoMapper;
using MoneyMind_BLL.DTOs;
using MoneyMind_BLL.DTOs.MonthlyGoals;
using MoneyMind_BLL.DTOs.SubWalletTypes;
using MoneyMind_BLL.DTOs.Transactions;
using MoneyMind_BLL.Services.Interfaces;
using MoneyMind_DAL.Entities;
using MoneyMind_DAL.Repositories.Implementations;
using MoneyMind_DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_BLL.Services.Implementations
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository transactionRepository;
        private readonly IMonthlyGoalRepository monthlyGoalRepository;
        private readonly IGoalItemService goalItemService;
        private readonly IGoalItemRepository goalItemRepository;
        private readonly IWalletRepository walletRepository;
        private readonly IWalletService walletService;
        private readonly ISubWalletTypeRepository subWalletTypeRepository;
        private readonly IMapper mapper;
        private readonly IMLService mlService;

        public TransactionService(ITransactionRepository transactionRepository,
            IMonthlyGoalRepository monthlyGoalRepository,
            IGoalItemService goalItemService,
            IGoalItemRepository goalItemRepository,
            IWalletRepository walletRepository,
            IWalletService walletService,
            ISubWalletTypeRepository subWalletTypeRepository,
            IMapper mapper, IMLService mlService)
        {
            this.transactionRepository = transactionRepository;
            this.monthlyGoalRepository = monthlyGoalRepository;
            this.goalItemService = goalItemService;
            this.goalItemRepository = goalItemRepository;
            this.walletRepository = walletRepository;
            this.walletService = walletService;
            this.subWalletTypeRepository = subWalletTypeRepository;
            this.mapper = mapper;
            this.mlService = mlService;
        }
        public async Task<TransactionResponse> AddTransactionAsync(Guid userId, TransactionRequest transactionRequest)
        {
            // Map or Convert DTO to Domain Model
            var transactionDomain = mapper.Map<Transaction>(transactionRequest);
            transactionDomain.UserId = userId;

            var tag = await mlService.ClassificationTag(
                transactionDomain.Description
            );
            transactionDomain.TagId = tag.Id;

            var monthlyGoal = await monthlyGoalRepository.GetCurrentGoalForUserAsync(userId, transactionDomain.TransactionDate);
            if (monthlyGoal != null)
            {
                if (transactionRequest.WalletId.HasValue)
                {
                    // Trừ tiền trong ví
                    await walletService.UpdateBalanceAsync(
                        transactionRequest.WalletId.Value,
                        -(double)transactionDomain.Amount  // amountDifference âm
                    );

                    // Lấy SubWalletTypeId để xác định loại ví
                    var wallet = await walletRepository.GetByIdAsync(transactionRequest.WalletId.Value);
                    var subWalletType = await subWalletTypeRepository.GetByIdAsync(wallet.SubWalletTypeId);

                    // Tăng UsedAmount và tính lại UsedPercentage trong GoalItem
                    await goalItemService.UpdateGoalItemAsync(
                        userId,
                        subWalletType.WalletTypeId,
                        monthlyGoal.Id,
                        (double)transactionDomain.Amount,       // amountDifference dương
                        (double)monthlyGoal.TotalAmount         // ép kiểu decimal -> double
                    );
                }
            }
            // Use Domain Model to create
            transactionDomain = await transactionRepository.InsertAsync(transactionDomain);

            return mapper.Map<TransactionResponse>(transactionDomain);
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
                var wallet = await walletRepository.GetByIdAsync(existingTransaction.WalletId.Value);
                var subWalletType = await subWalletTypeRepository.GetByIdAsync(wallet.SubWalletTypeId);

                await goalItemService.UpdateGoalItemAsync(
                    userId,
                    subWalletType.WalletTypeId,
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
            var transactions = response.Item1;
            var totalPages = response.Item2;
            var totalRecords = response.Item3;

            // Giả sử authorDomains là danh sách các đối tượng AuthorDomain
            var transactionResponse = mapper.Map<List<TransactionResponse>>(transactions);

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
            var transaction = await transactionRepository.GetByIdAsync(transactionId, t => t.Tag);

            if (transaction == null)
            {
                return null;
            }

            return mapper.Map<TransactionResponse>(transaction);
        }

        public async Task<TransactionResponse> UpdateTransactionAsync(Guid transactionId, Guid userId, TransactionRequest transactionRequest)
        {
            var existingTransaction = await transactionRepository.GetByIdAsync(transactionId, t => t.Wallet.SubWalletType);
            if (existingTransaction == null || existingTransaction.UserId != userId)
            {
                return null;
            }

            var amountDifference = transactionRequest.Amount - existingTransaction.Amount;
            var monthlyGoal = await monthlyGoalRepository.GetCurrentGoalForUserAsync(userId, existingTransaction.TransactionDate);

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
                        var oldSubWalletType = await subWalletTypeRepository.GetByIdAsync(oldWallet.SubWalletTypeId);

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
                        var newWallet = await walletRepository.GetByIdAsync(transactionRequest.WalletId.Value);
                        var newSubWalletType = await subWalletTypeRepository.GetByIdAsync(newWallet.SubWalletTypeId);

                        await goalItemService.UpdateGoalItemAsync(
                            userId,
                            newSubWalletType.WalletTypeId,
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
                    var subWalletType = await subWalletTypeRepository.GetByIdAsync(wallet.SubWalletTypeId);

                    await goalItemService.UpdateGoalItemAsync(
                        userId,
                        subWalletType.WalletTypeId,
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
            existingTransaction.UpdatedAt = DateTime.Now;

            // Cập nhật tag bằng AI
            var tag = await mlService.ClassificationTag(existingTransaction.Description);
            existingTransaction.TagId = tag.Id;

            existingTransaction = await transactionRepository.UpdateAsync(existingTransaction);

            return mapper.Map<TransactionResponse>(existingTransaction);
        }

    }
}
