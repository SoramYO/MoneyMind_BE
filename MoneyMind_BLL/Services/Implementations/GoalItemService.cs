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
        private readonly ITransactionRepository transactionRepository;
        private readonly IMonthlyGoalRepository monthlyGoalRepository;
        private readonly IMapper mapper;

        public GoalItemService(
            IGoalItemRepository goalItemRepository,
            ITransactionRepository transactionRepository,
            IMonthlyGoalRepository monthlyGoalRepository,
            IMapper mapper)
        {
            this.goalItemRepository = goalItemRepository;
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
            // Lấy GoalItem hiện tại
            var existingGoalItem = await goalItemRepository.GetByIdAsync(goalItemId, g => g.MonthlyGoal);
            if (existingGoalItem == null) return null;

            // Không cho phép thay đổi WalletTypeId và MonthlyGoalId
            if (existingGoalItem.WalletTypeId != goalItemRequest.WalletTypeId || existingGoalItem.MonthlyGoalId != goalItemRequest.MonthlyGoalId)
            {
                return null;
            }

            // Chỉ cập nhật các trường được phép thay đổi
            existingGoalItem.Description = goalItemRequest.Description;
            existingGoalItem.MinTargetPercentage = goalItemRequest.MinTargetPercentage;
            existingGoalItem.MaxTargetPercentage = goalItemRequest.MaxTargetPercentage;
            existingGoalItem.MinAmount = goalItemRequest.MinAmount;
            existingGoalItem.MaxAmount = goalItemRequest.MaxAmount;
            existingGoalItem.TargetMode = goalItemRequest.TargetMode;

            // Cập nhật trạng thái IsAchieved
            UpdateIsAchieved(existingGoalItem);

            // Cập nhật GoalItem trong cơ sở dữ liệu
            existingGoalItem = await goalItemRepository.UpdateAsync(existingGoalItem);

            // Cập nhạt Trạng thái của MonthlyGoal
            await UpdateGoalStatusAsync(existingGoalItem.MonthlyGoalId);

            return mapper.Map<GoalItemResponse>(existingGoalItem);
        }

        public async Task UpdateGoalItemAsync(Guid userId, Guid walletTypeId, Guid monthlyGoalId, double amountDifference, double totalAmount)
        {
            var goalItem = await goalItemRepository.GetByWalletTypeAsync(userId, walletTypeId, monthlyGoalId);
            if (goalItem != null)
            {
                goalItem.UsedAmount += amountDifference;
                goalItem.UsedPercentage = (goalItem.UsedAmount / totalAmount) * 100;

                // Cập nhật trạng thái đạt mục tiêu
                UpdateIsAchieved(goalItem);

                await goalItemRepository.UpdateAsync(goalItem);
            }
            await UpdateGoalStatusAsync(monthlyGoalId);
        }
        public async Task UpdateGoalStatusAsync(Guid monthlyGoalId)
        {
            var monthlyGoal = await monthlyGoalRepository.GetByIdAsync(monthlyGoalId);
            if (monthlyGoal == null) return;

            bool allAchieved = true;

            foreach (var goalItem in monthlyGoal.GoalItems)
            {
                if (!goalItem.IsAchieved)
                {
                    allAchieved = false;
                    break;
                }
            }

            monthlyGoal.IsCompleted = allAchieved;
            monthlyGoal.Status = allAchieved ? GoalStatus.Completed : GoalStatus.InProgress;

            await monthlyGoalRepository.UpdateAsync(monthlyGoal);
        }
        public void UpdateIsAchieved(GoalItem goalItem)
        {
            switch (goalItem.TargetMode)
            {
                case TargetMode.MaxOnly:
                    goalItem.IsAchieved = goalItem.UsedAmount >= goalItem.MaxAmount;
                    break;
                case TargetMode.MinOnly:
                    goalItem.IsAchieved = goalItem.UsedAmount >= goalItem.MinAmount;
                    break;
                case TargetMode.Range:
                    goalItem.IsAchieved = goalItem.UsedAmount >= goalItem.MinAmount &&
                                          goalItem.UsedAmount <= goalItem.MaxAmount;
                    break;
                case TargetMode.PercentageOnly:
                    goalItem.IsAchieved = goalItem.UsedPercentage >= goalItem.MinTargetPercentage &&
                                          goalItem.UsedPercentage <= goalItem.MaxTargetPercentage;
                    break;
                case TargetMode.FixedAmount:
                    goalItem.IsAchieved = goalItem.UsedAmount == goalItem.MinAmount;
                    break;
                case TargetMode.NoTarget:
                    goalItem.IsAchieved = false;
                    break;
            }
        }
    }
}
