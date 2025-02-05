using AutoMapper;
using MoneyMind_BLL.DTOs;
using MoneyMind_BLL.DTOs.MonthlyGoals;
using MoneyMind_BLL.DTOs.SubWalletTypes;
using MoneyMind_BLL.Services.Interfaces;
using MoneyMind_DAL.Entities;
using MoneyMind_DAL.Repositories.Implementations;
using MoneyMind_DAL.Repositories.Interfaces;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_BLL.Services.Implementations
{
    public class MonthlyGoalService : IMonthlyGoalService
    {
        private readonly IMonthlyGoalRepository monthlyGoalRepository;
        private readonly ITransactionRepository transactionRepository;
        private readonly IGoalItemRepository goalItemRepository;
        private readonly IGoalItemService goalItemService;
        private readonly IMapper mapper;

        public MonthlyGoalService(IMonthlyGoalRepository monthlyGoalRepository,
            ITransactionRepository transactionRepository,
            IGoalItemRepository goalItemRepository,
            IGoalItemService goalItemService,
            IMapper mapper)
        {
            this.monthlyGoalRepository = monthlyGoalRepository;
            this.transactionRepository = transactionRepository;
            this.goalItemRepository = goalItemRepository;
            this.goalItemService = goalItemService;
            this.mapper = mapper;
        }

        public async Task<MonthlyGoalResponse> AddMonthlyGoalAsync(Guid userId, MonthlyGoalRequest monthlyGoalRequest)
        {
            // Map DTO sang Domain Model
            var monthlyGoalDomain = mapper.Map<MonthlyGoal>(monthlyGoalRequest);
            monthlyGoalDomain.UserId = userId;

            // Danh sách WalletTypeId mặc định
            var defaultWalletTypeIds = new List<Guid>
            {
                Guid.Parse("B79D14DB-7A81-4046-B66E-1ACD761123BB"),
                Guid.Parse("B203AE2F-3023-41C1-A25A-2B2EC238321D"),
                Guid.Parse("6193FCB1-C8C4-44E9-ABDE-78CDB4258C4E"),
                Guid.Parse("654A9673-4D23-44B1-9AF8-A9562341A60E"),
                Guid.Parse("19EA7E67-8095-4A13-BBA4-BDA0A4A47A38"),
                Guid.Parse("EBEBC667-520D-4EAC-88ED-EF9EB8E26AAB")
            };

            var goalItems = new List<GoalItem>();

            foreach (var walletTypeId in defaultWalletTypeIds)
            {
                var usedAmount = await transactionRepository.GetSumAsync(
                    t => t.UserId == userId
                      && t.Wallet != null
                      && t.Wallet.SubWalletType != null
                      && t.Wallet.SubWalletType.WalletTypeId == walletTypeId
                      && t.TransactionDate.Month == monthlyGoalDomain.Month
                      && t.TransactionDate.Year == monthlyGoalDomain.Year,
                    t => t.Amount
                );

                var goalItem = new GoalItem
                {
                    Id = Guid.NewGuid(),
                    MonthlyGoalId = monthlyGoalDomain.Id,
                    WalletTypeId = walletTypeId,
                    MinTargetPercentage = 0,  // Mặc định có thể thay đổi
                    MaxTargetPercentage = 100, // Mặc định có thể thay đổi
                    MinAmount = 0,  // Giá trị tối thiểu mặc định
                    MaxAmount = monthlyGoalDomain.TotalAmount / defaultWalletTypeIds.Count, // Chia đều ngân sách
                    TargetMode = TargetMode.PercentageOnly, // Hoặc đặt mặc định là Absolute
                    IsAchieved = false,
                    UsedAmount = usedAmount,
                    UsedPercentage = (monthlyGoalDomain.TotalAmount > 0) ? (usedAmount / monthlyGoalDomain.TotalAmount) * 100 : 0
                };

                goalItems.Add(goalItem);
            }

            monthlyGoalDomain.GoalItems = goalItems;

            monthlyGoalDomain = await monthlyGoalRepository.InsertAsync(monthlyGoalDomain);

            return mapper.Map<MonthlyGoalResponse>(monthlyGoalDomain);
        }


        public async Task<ListDataResponse> GetMonthlyGoalAsync(Expression<Func<MonthlyGoal, bool>>? filter, Func<IQueryable<MonthlyGoal>, IOrderedQueryable<MonthlyGoal>> orderBy, string includeProperties, int pageIndex, int pageSize)
        {
            var response = await monthlyGoalRepository.GetAsync(
                        filter: filter,
                        orderBy: orderBy,
                        includeProperties: includeProperties,
                        pageIndex: pageIndex,
                        pageSize: pageSize
                        );
            var monthlyGoals = response.Item1;
            var totalPages = response.Item2;
            var totalRecords = response.Item3;

            // Giả sử authorDomains là danh sách các đối tượng AuthorDomain
            var monthlyGoalResponse = mapper.Map<List<MonthlyGoalResponse>>(monthlyGoals);

            var listResponse = new ListDataResponse
            {
                TotalRecord = totalRecords,
                TotalPage = totalPages,
                PageIndex = pageIndex,
                Data = monthlyGoalResponse
            };

            return listResponse;
        }

        public async Task<MonthlyGoalResponse> GetMonthlyGoalByIdAsync(Guid monthlyGoalId)
        {
            var monthlyGoal = await monthlyGoalRepository.GetByIdAsync(monthlyGoalId, g => g.GoalItems);

            if (monthlyGoal == null)
            {
                return null;
            }

            return mapper.Map<MonthlyGoalResponse>(monthlyGoal);
        }

        public async Task UpdateGoalItemPercentages(MonthlyGoal monthlyGoal)
        {
            if (monthlyGoal.GoalItems != null && monthlyGoal.GoalItems.Any())
            {
                foreach (var goalItem in monthlyGoal.GoalItems)
                {
                    goalItem.UsedPercentage = monthlyGoal.TotalAmount > 0
                        ? (goalItem.UsedAmount / monthlyGoal.TotalAmount) * 100
                        : 0; // Tránh chia cho 0

                    goalItemService.UpdateIsAchieved(goalItem);
                    await goalItemRepository.UpdateAsync(goalItem);
                }
                await UpdateGoalStatusAsync(monthlyGoal.Id);
            }
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
            monthlyGoal.Status = allAchieved ? GoalStatus.Completed :  GoalStatus.InProgress;

            await monthlyGoalRepository.UpdateAsync(monthlyGoal);
        }

        public async Task<MonthlyGoalResponse> UpdateMonthlyGoalAsync(Guid monthlyGoalId, Guid userId, MonthlyGoalRequest monthlyGoalRequest)
        {
            var existingMonthlyGoal = await monthlyGoalRepository.GetByIdAsync(monthlyGoalId, s => s.GoalItems);

            if (existingMonthlyGoal == null || existingMonthlyGoal.UserId != userId)
            {
                return null;
            }

            bool isTotalAmountChanged = existingMonthlyGoal.TotalAmount != monthlyGoalRequest.TotalAmount;

            // Chỉ cập nhật các trường được phép thay đổi
            existingMonthlyGoal.TotalAmount = monthlyGoalRequest.TotalAmount;

            if (isTotalAmountChanged)
            {
                await UpdateGoalItemPercentages(existingMonthlyGoal);
            }

            await monthlyGoalRepository.UpdateAsync(existingMonthlyGoal);

            return mapper.Map<MonthlyGoalResponse>(existingMonthlyGoal);
        }

    }
}
