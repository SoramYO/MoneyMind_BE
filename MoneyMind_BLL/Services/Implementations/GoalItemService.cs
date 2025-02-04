using AutoMapper;
using MoneyMind_BLL.DTOs.GoalItems;
using MoneyMind_BLL.DTOs;
using MoneyMind_BLL.Services.Interfaces;
using MoneyMind_DAL.Entities;
using MoneyMind_DAL.Repositories.Interfaces;
using System.Linq.Expressions;

namespace MoneyMind_BLL.Services.Implementations
{
    public class GoalItemService : IGoalItemService
    {
        private readonly IGoalItemRepository goalItemRepository;
        private readonly IWalletRepository walletRepository;
        private readonly ITransactionRepository transactionRepository;
        private readonly IMonthlyGoalRepository monthlyGoalRepository;
        private readonly IMapper mapper;

        public GoalItemService(
            IGoalItemRepository goalItemRepository,
            IWalletRepository walletRepository,
            ITransactionRepository transactionRepository,
            IMonthlyGoalRepository monthlyGoalRepository,
            IMapper mapper)
        {
            this.goalItemRepository = goalItemRepository;
            this.walletRepository = walletRepository;
            this.transactionRepository = transactionRepository;
            this.monthlyGoalRepository = monthlyGoalRepository;
            this.mapper = mapper;
        }
        public async Task<GoalItemResponse> AddGoalItemAsync(Guid userId, GoalItemRequest goalItemRequest)
        {
            var existGoalItem = goalItemRepository.GetByWalletTypeAsync(userId, goalItemRequest.WalletTypeId, goalItemRequest.MonthlyGoalId);
            var goalItemDomain = mapper.Map<GoalItem>(goalItemRequest);
            var monthlyGoal = await monthlyGoalRepository.GetByIdAsync(goalItemRequest.MonthlyGoalId);
            if (monthlyGoal == null || monthlyGoal.UserId != userId || existGoalItem != null)
            {
                return null;
            }
            double usedAmount = await CalculateUsedAmountForNewGoalItemAsync(
                monthlyGoal.UserId,
                goalItemRequest.WalletTypeId,
                monthlyGoal.Month,
                monthlyGoal.Year
            );

            goalItemDomain.UsedAmount = usedAmount;
            goalItemDomain.UsedPercentage = monthlyGoal.TotalAmount > 0
                ? (usedAmount / monthlyGoal.TotalAmount) * 100
                : 0;

            goalItemDomain = await goalItemRepository.InsertAsync(goalItemDomain);
            return mapper.Map<GoalItemResponse>(goalItemDomain);
        }

        public async Task<double> CalculateUsedAmountForNewGoalItemAsync(Guid userId, Guid walletTypeId, int month, int year)
        {
            return await transactionRepository
                .GetSumAsync(t => t.UserId == userId
                               && t.Wallet != null
                               && t.Wallet.SubWalletType != null
                               && t.Wallet.SubWalletType.WalletTypeId == walletTypeId
                               && t.TransactionDate.Month == month
                               && t.TransactionDate.Year == year,
                             t => t.Amount);
        }

        public async Task<ListDataResponse> GetGoalItemAsync(
            Expression<Func<GoalItem, bool>>? filter,
            Func<IQueryable<GoalItem>, IOrderedQueryable<GoalItem>> orderBy,
            string includeProperties,
            int pageIndex,
            int pageSize)
        {
            var response = await goalItemRepository.GetAsync(
                filter: filter,
                orderBy: orderBy,
                includeProperties: includeProperties,
                pageIndex: pageIndex,
                pageSize: pageSize
            );

            var goalItemsResponse = mapper.Map<List<GoalItemResponse>>(response.Item1);

            return new ListDataResponse
            {
                TotalRecord = response.Item3,
                TotalPage = response.Item2,
                PageIndex = pageIndex,
                Data = goalItemsResponse
            };
        }

        public async Task<GoalItemResponse> GetGoalItemByIdAsync(Guid goalItemId)
        {
            var goalItem = await goalItemRepository.GetByIdAsync(goalItemId, s => s.MonthlyGoal);
            return goalItem == null ? null : mapper.Map<GoalItemResponse>(goalItem);
        }

        public async Task<GoalItemResponse> UpdateGoalItemAsync(Guid goalItemId, Guid userId, GoalItemRequest goalItemRequest)
        {
            var existGoalItem = goalItemRepository.GetByWalletTypeAsync(userId, goalItemRequest.WalletTypeId, goalItemRequest.MonthlyGoalId);

            var existingGoalItem = await goalItemRepository.GetByIdAsync(goalItemId);
            if (existingGoalItem == null || existGoalItem != null) return null;

            var oldWalletTypeId = existingGoalItem.WalletTypeId;
            var oldMonthlyGoalId = existingGoalItem.MonthlyGoalId;

            existingGoalItem.Description = goalItemRequest.Description;
            existingGoalItem.MinTargetPercentage = goalItemRequest.MinTargetPercentage;
            existingGoalItem.MaxTargetPercentage = goalItemRequest.MaxTargetPercentage;
            existingGoalItem.MinAmount = goalItemRequest.MinAmount;
            existingGoalItem.MaxAmount = goalItemRequest.MaxAmount;
            existingGoalItem.TargetMode = goalItemRequest.TargetMode;
            existingGoalItem.IsAchieved = goalItemRequest.IsAchieved;
            existingGoalItem.MonthlyGoalId = goalItemRequest.MonthlyGoalId;
            existingGoalItem.WalletTypeId = goalItemRequest.WalletTypeId;

            if (oldWalletTypeId != goalItemRequest.WalletTypeId)
            {
                // Tính lại UsedAmount dựa trên các giao dịch mới
                double newUsedAmount = await transactionRepository.GetSumAsync(
                    t => t.UserId == userId
                      && t.Wallet != null
                      && t.Wallet.SubWalletType != null
                      && t.Wallet.SubWalletType.WalletTypeId == goalItemRequest.WalletTypeId
                      && t.TransactionDate.Month == existingGoalItem.MonthlyGoal.Month
                      && t.TransactionDate.Year == existingGoalItem.MonthlyGoal.Year,
                    t => t.Amount
                );

                existingGoalItem.UsedAmount = newUsedAmount;
            }

            if (oldMonthlyGoalId != goalItemRequest.MonthlyGoalId)
            {
                // Cập nhật UsedAmount trong MonthlyGoal cũ
                var oldMonthlyGoal = await monthlyGoalRepository.GetByIdAsync(oldMonthlyGoalId);
                if (oldMonthlyGoal != null)
                {
                    oldMonthlyGoal.TotalAmount -= existingGoalItem.UsedAmount;
                    await monthlyGoalRepository.UpdateAsync(oldMonthlyGoal);
                }

                // Cập nhật UsedAmount trong MonthlyGoal mới
                var newMonthlyGoal = await monthlyGoalRepository.GetByIdAsync(goalItemRequest.MonthlyGoalId);
                if (newMonthlyGoal != null)
                {
                    newMonthlyGoal.TotalAmount += existingGoalItem.UsedAmount;
                    await monthlyGoalRepository.UpdateAsync(newMonthlyGoal);
                }
            }

            existingGoalItem = await goalItemRepository.UpdateAsync(existingGoalItem);
            return mapper.Map<GoalItemResponse>(existingGoalItem);
        }

        public async Task UpdateUsedAmountOnNewTransactionAsync(Transaction transaction)
        {
            if (transaction.WalletId == null)
            {
                throw new ArgumentNullException(nameof(transaction.WalletId), "WalletId cannot be null");
            }

            var wallet = await walletRepository.GetByIdAsync(transaction.WalletId);
            if (wallet == null) return;

            Guid walletTypeId = wallet.SubWalletType.WalletTypeId;
            Guid userId = transaction.UserId;
            int month = transaction.TransactionDate.Month;
            int year = transaction.TransactionDate.Year;

            var goalItems = await goalItemRepository.GetAsync(
                filter: g => g.MonthlyGoal.UserId == userId
                          && g.WalletTypeId == walletTypeId
                          && g.MonthlyGoal.Month == month
                          && g.MonthlyGoal.Year == year,
                includeProperties: "MonthlyGoal"
            );

            foreach (var goalItem in goalItems.Item1)
            {
                goalItem.UsedAmount += transaction.Amount;
                goalItem.UsedPercentage = goalItem.MonthlyGoal.TotalAmount > 0
                    ? (goalItem.UsedAmount / goalItem.MonthlyGoal.TotalAmount) * 100
                    : 0;

                await goalItemRepository.UpdateAsync(goalItem);
            }
        }

        public async Task RecalculateUsedAmountAsync(Guid userId, Guid walletTypeId, int month, int year)
        {
            var goalItems = await goalItemRepository.GetAsync(
                filter: g => g.MonthlyGoal.UserId == userId
                          && g.WalletTypeId == walletTypeId
                          && g.MonthlyGoal.Month == month
                          && g.MonthlyGoal.Year == year,
                includeProperties: "MonthlyGoal"
            );

            double totalUsedAmount = await transactionRepository.GetSumAsync(
                t => t.UserId == userId
                  && t.Wallet != null
                  && t.Wallet.SubWalletType != null
                  && t.Wallet.SubWalletType.WalletTypeId == walletTypeId
                  && t.TransactionDate.Month == month
                  && t.TransactionDate.Year == year,
                t => t.Amount
            );

            foreach (var goalItem in goalItems.Item1)
            {
                goalItem.UsedAmount = totalUsedAmount;
                goalItem.UsedPercentage = goalItem.MonthlyGoal.TotalAmount > 0
                    ? (totalUsedAmount / goalItem.MonthlyGoal.TotalAmount) * 100
                    : 0;

                await goalItemRepository.UpdateAsync(goalItem);
            }
        }
    }
}
