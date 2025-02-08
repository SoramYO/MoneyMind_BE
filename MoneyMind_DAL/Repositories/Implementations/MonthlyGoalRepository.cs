using Microsoft.EntityFrameworkCore;
using MoneyMind_DAL.DBContexts;
using MoneyMind_DAL.Entities;
using MoneyMind_DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_DAL.Repositories.Implementations
{
    public class MonthlyGoalRepository : GenericRepository<MonthlyGoal>, IMonthlyGoalRepository
    {
        public MonthlyGoalRepository(MoneyMindDbContext context) : base(context)
        {
        }

        public async Task<MonthlyGoal> GetCurrentGoalForUserAsync(Guid userId, DateTime transactionDate)
        {
            var monthlyGoal = await _dbSet
                .Include(m => m.GoalItems) // Load danh sách GoalItems nếu có
                .FirstOrDefaultAsync(m => m.UserId == userId &&
                                          m.Year == transactionDate.Year &&
                                          m.Month == transactionDate.Month);

            if (monthlyGoal == null)
            {
                monthlyGoal = new MonthlyGoal
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Year = transactionDate.Year,
                    Month = transactionDate.Month,
                    TotalAmount = 6000000, // Giá trị mặc định hoặc có thể lấy từ cấu hình
                    Status = GoalStatus.InProgress, // Chỉnh sửa theo enum phù hợp
                    IsCompleted = false,
                    GoalItems = new List<GoalItem>()
                };

                var defaultWalletTypeIds = new List<Guid>
                {
                    Guid.Parse("B79D14DB-7A81-4046-B66E-1ACD761123BB"),
                    Guid.Parse("B203AE2F-3023-41C1-A25A-2B2EC238321D"),
                    Guid.Parse("6193FCB1-C8C4-44E9-ABDE-78CDB4258C4E"),
                    Guid.Parse("654A9673-4D23-44B1-9AF8-A9562341A60E"),
                    Guid.Parse("19EA7E67-8095-4A13-BBA4-BDA0A4A47A38"),
                    Guid.Parse("EBEBC667-520D-4EAC-88ED-EF9EB8E26AAB")
                };

                foreach (var walletTypeId in defaultWalletTypeIds)
                {
                    var usedAmount = await _context.Transactions
                        .Where(t => t.UserId == userId
                                    && t.Wallet.SubWalletType.WalletTypeId == walletTypeId
                                    && t.TransactionDate.Year == transactionDate.Year
                                    && t.TransactionDate.Month == transactionDate.Month)
                        .SumAsync(t => t.Amount);

                    var goalItem = new GoalItem
                    {
                        Id = Guid.NewGuid(),
                        Description = $"Mục tiêu mặc định cho {transactionDate.Month}-{transactionDate.Year}",
                        MonthlyGoalId = monthlyGoal.Id,
                        WalletTypeId = walletTypeId,
                        MinTargetPercentage = 0,
                        MaxTargetPercentage = 100,
                        MinAmount = 0,
                        MaxAmount = monthlyGoal.TotalAmount / defaultWalletTypeIds.Count,
                        TargetMode = TargetMode.NoTarget,
                        IsAchieved = false,
                        UsedAmount = usedAmount,
                        UsedPercentage = (monthlyGoal.TotalAmount > 0) ? (usedAmount / monthlyGoal.TotalAmount) * 100 : 0
                    };

                    monthlyGoal.GoalItems.Add(goalItem);
                }

                _dbSet.Add(monthlyGoal);
                await _context.SaveChangesAsync();
            }

            return monthlyGoal;
        }

    }
}
