using Microsoft.EntityFrameworkCore;
using MoneyMind_DAL.DBContexts;
using MoneyMind_DAL.Entities;
using MoneyMind_DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
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
            return monthlyGoal;
        }

    }
}
