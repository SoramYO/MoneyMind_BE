using MoneyMind_BLL.DTOs;
using MoneyMind_BLL.DTOs.Activities;
using MoneyMind_BLL.DTOs.GoalItems;
using MoneyMind_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_BLL.Services.Interfaces
{
    public interface IActivityService
    {
        Task<ListDataResponse> GetActivityAsync(
            Expression<Func<Activity, bool>>? filter,
            Func<IQueryable<Activity>, IOrderedQueryable<Activity>> orderBy,
            string includeProperties,
            int pageIndex,
            int pageSize);
        Task<ActivityResponse> AddActivityAsync(Guid userId, ActivityRequest activityRequest);
        Task<ActivityResponse> UpdateActivityAsync(Guid userId, Guid activityId, ActivityRequest activityRequest);
        Task<ActivityResponse> DeleteActivityAsync(Guid userId, Guid activityId);
    }
}
