using AutoMapper;
using MoneyMind_BLL.DTOs;
using MoneyMind_BLL.DTOs.DataDefaults;
using MoneyMind_BLL.DTOs.MonthlyGoals;
using MoneyMind_BLL.Services.Interfaces;
using MoneyMind_DAL.Entities;
using MoneyMind_DAL.Repositories.Interfaces;
using System.Linq.Expressions;
using System.Text.Json;

namespace MoneyMind_BLL.Services.Implementations
{
    public class MonthlyGoalService : IMonthlyGoalService
    {
        private readonly IWalletTypeRepository walletTypeRepository;
        private readonly IMonthlyGoalRepository monthlyGoalRepository;
        private readonly ITransactionRepository transactionRepository;
        private readonly IGoalItemRepository goalItemRepository;
        private readonly IGoalItemService goalItemService;
        private readonly INotificationService _notificationService;

        private readonly IMapper mapper;

        public MonthlyGoalService(
            IWalletTypeRepository walletTypeRepository,
            IMonthlyGoalRepository monthlyGoalRepository,
            ITransactionRepository transactionRepository,
            IGoalItemRepository goalItemRepository,
            IGoalItemService goalItemService,
            INotificationService notificationService,
            IMapper mapper)
        {
            this.walletTypeRepository = walletTypeRepository;
            this.monthlyGoalRepository = monthlyGoalRepository;
            this.transactionRepository = transactionRepository;
            this.goalItemRepository = goalItemRepository;
            this.goalItemService = goalItemService;
            this._notificationService = notificationService;
            this.mapper = mapper;
        }

        public async Task<MonthlyGoalResponse> AddMonthlyGoalAsync(Guid userId, MonthlyGoalRequest monthlyGoalRequest)
        {
            var jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "datadefaults.json");
            var jsonString = await File.ReadAllTextAsync(jsonFilePath);
            var dataDefaults = JsonSerializer.Deserialize<DataDefaults>(jsonString);

            var goalDefaults = dataDefaults.GoalItem;

            // Map DTO sang Domain Model
            var monthlyGoalDomain = mapper.Map<MonthlyGoal>(monthlyGoalRequest);
            monthlyGoalDomain.UserId = userId;

            var walletTypedIds = (await walletTypeRepository.GetAsync(filter: w => w.IsDisabled == false))
                                               .Item1
                                               .Select(item => item.Id);

            var goalItems = new List<GoalItem>();

            foreach (var walletTypeId in walletTypedIds)
            {
                //var usedAmount = await transactionRepository.GetSumAsync(
                //    t => t.UserId == userId
                //      && t.Wallet != null
                //      && t.Wallet.WalletCategory != null
                //      && t.Wallet.WalletCategory.WalletTypeId == walletTypeId
                //      && t.TransactionDate.Month == monthlyGoalDomain.Month
                //      && t.TransactionDate.Year == monthlyGoalDomain.Year,
                //    t => t.Amount
                //);

                var goalItem = new GoalItem
                {
                    Id = Guid.NewGuid(),
                    Description = goalDefaults.Description,
                    MonthlyGoalId = monthlyGoalDomain.Id,
                    WalletTypeId = walletTypeId,
                    MinTargetPercentage = goalDefaults.MinTargetPercentage,  // Mặc định có thể thay đổi
                    MaxTargetPercentage = goalDefaults.MaxTargetPercentage, // Mặc định có thể thay đổi
                    MinAmount = goalDefaults.MinAmount,  // Giá trị tối thiểu mặc định
                    MaxAmount = goalDefaults.MaxAmount, // Chia đều ngân sách
                    //UsedAmount = usedAmount,
                    //UsedPercentage = (monthlyGoalDomain.TotalAmount > 0) ? (usedAmount / monthlyGoalDomain.TotalAmount) * 100 : 0
                    UsedAmount = 0,
                    UsedPercentage = 0,
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
            var monthlyGoal = await monthlyGoalRepository.GetByIdAsync(monthlyGoalId, g => g.GoalItems);
            if (monthlyGoal == null) return;

            bool allAchieved = true;
            bool previouslyCompleted = monthlyGoal.IsCompleted;

            foreach (var goalItem in monthlyGoal.GoalItems)
            {
                if (!goalItem.IsAchieved)
                {
                    allAchieved = false;
                    break;
                }
            }

            // Check if the status changed from incomplete to complete
            if (allAchieved && !previouslyCompleted)
            {
                // Send notification only when status changes from incomplete to complete
                await _notificationService.SendNotificationToUser(
                    monthlyGoal.UserId,
                    $"Congratulations! You've achieved all your financial goals for {GetMonthName(monthlyGoal.Month)} {monthlyGoal.Year}.",
                    "monthly_goal_completion"
                );
            }

            monthlyGoal.IsCompleted = allAchieved;
            monthlyGoal.Status = allAchieved ? GoalStatus.Completed : GoalStatus.InProgress;

            await monthlyGoalRepository.UpdateAsync(monthlyGoal);
        }

        private string GetMonthName(int month)
        {
            return new DateTime(2022, month, 1).ToString("MMMM");
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
