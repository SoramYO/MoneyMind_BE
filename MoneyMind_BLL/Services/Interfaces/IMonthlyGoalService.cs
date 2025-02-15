using MoneyMind_BLL.DTOs.WalletCategories;
using MoneyMind_BLL.DTOs;
using MoneyMind_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MoneyMind_BLL.DTOs.MonthlyGoals;

namespace MoneyMind_BLL.Services.Interfaces
{
    public interface IMonthlyGoalService
    {
        Task<ListDataResponse> GetMonthlyGoalAsync(
            Expression<Func<MonthlyGoal, bool>>? filter,
            Func<IQueryable<MonthlyGoal>, IOrderedQueryable<MonthlyGoal>> orderBy,
            string includeProperties,
            int pageIndex,
            int pageSize);
        Task<MonthlyGoalResponse> AddMonthlyGoalAsync(Guid userId, MonthlyGoalRequest monthlyGoalRequest);
        Task<MonthlyGoalResponse> UpdateMonthlyGoalAsync(Guid monthlyGoalId, Guid userId, MonthlyGoalRequest monthlyGoalRequest);
        Task<MonthlyGoalResponse> GetMonthlyGoalByIdAsync(Guid monthlyGoalId);
        Task UpdateGoalItemPercentages(MonthlyGoal monthlyGoal);
        Task UpdateGoalStatusAsync(Guid monthlyGoalId);
    }
}
