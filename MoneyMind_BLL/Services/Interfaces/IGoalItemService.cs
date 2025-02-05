using MoneyMind_BLL.DTOs.Transactions;
using MoneyMind_BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MoneyMind_DAL.Entities;
using MoneyMind_BLL.DTOs.GoalItems;

namespace MoneyMind_BLL.Services.Interfaces
{
    public interface IGoalItemService
    {
        Task<ListDataResponse> GetGoalItemAsync(
            Expression<Func<GoalItem, bool>>? filter,
            Func<IQueryable<GoalItem>, IOrderedQueryable<GoalItem>> orderBy,
            string includeProperties,
            int pageIndex,
            int pageSize);
        Task<GoalItemResponse> AddGoalItemAsync(Guid userId, GoalItemRequest goalItemRequest);
        Task<GoalItemResponse> UpdateGoalItemAsync(Guid goalItemId, Guid userId, GoalItemRequest goalItemRequest);
        Task<GoalItemResponse> GetGoalItemByIdAsync(Guid goalItemId);
        Task<double> CalculateUsedAmountForNewGoalItemAsync(Guid userId, Guid walletTypeId, int month, int year);
        Task UpdateGoalItemAsync(Guid userId, Guid walletTypeId, Guid monthlyGoalId, double amountDifference, double totalAmount);
        void UpdateIsAchieved(GoalItem goalItem);
    }
}
