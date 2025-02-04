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
            .Where(m => m.UserId == userId &&
                        m.Year == transactionDate.Year &&
                        m.Month == transactionDate.Month)
            .FirstOrDefaultAsync();

            if (monthlyGoal == null)
            {
                monthlyGoal = new MonthlyGoal
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Year = transactionDate.Year,
                    Month = transactionDate.Month,
                    TotalAmount = 3000000, // Hoặc giá trị mặc định khác
                    Status = (GoalStatus)0, // Hoặc giá trị phù hợp với logic
                    IsCompleted = false,
                };

                _dbSet.Add(monthlyGoal);
                await _context.SaveChangesAsync();
            }

            return monthlyGoal;
        }
    }
}
