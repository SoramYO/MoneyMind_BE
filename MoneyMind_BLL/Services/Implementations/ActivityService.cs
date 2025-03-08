using AutoMapper;
using MoneyMind_BLL.DTOs.Activities;
using MoneyMind_BLL.DTOs;
using MoneyMind_DAL.Repositories.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MoneyMind_DAL.Entities;
using MoneyMind_DAL.Repositories.Interfaces;
using MoneyMind_BLL.Services.Interfaces;
using MoneyMind_BLL.DTOs.Wallets;

namespace MoneyMind_BLL.Services.Implementations
{
    public class ActivityService : IActivityService
    {
        private readonly IActivityRepository activityRepository;
        private readonly IMapper mapper;

        public ActivityService(IActivityRepository activityRepository, IMapper mapper)
        {
            this.activityRepository = activityRepository;
            this.mapper = mapper;
        }

        public async Task<ActivityResponse> AddActivityAsync(Guid userId, ActivityRequest activityRequest)
        {
            // Map or Convert DTO to Domain Model
            var activityDomain = mapper.Map<Activity>(activityRequest);
            // Use Domain Model to create Author
            activityDomain = await activityRepository.InsertAsync(activityDomain);

            return mapper.Map<ActivityResponse>(activityDomain);
        }

        public async Task<ActivityResponse> DeleteActivityAsync(Guid userId, Guid activityId)
        {
            var existingActivity = await activityRepository.GetByIdAsync(activityId, a => a.WalletCategory);
            if (existingActivity == null || existingActivity.WalletCategory.UserId != userId)
            {
                return null;
            }

            existingActivity.IsDeleted = true;

            existingActivity = await activityRepository.UpdateAsync(existingActivity);

            return mapper.Map<ActivityResponse>(existingActivity);
        }

        public async Task<ListDataResponse> GetActivityAsync(Expression<Func<Activity, bool>>? filter, Func<IQueryable<Activity>, IOrderedQueryable<Activity>> orderBy, string includeProperties, int pageIndex, int pageSize)
        {
            var response = await activityRepository.GetAsync(
                        filter: filter,
                        orderBy: orderBy,
                        includeProperties: includeProperties,
                        pageIndex: pageIndex,
                        pageSize: pageSize
                        );
            var activities = response.Item1;
            var totalPages = response.Item2;
            var totalRecords = response.Item3;

            // Giả sử authorDomains là danh sách các đối tượng AuthorDomain
            var activityResponses = mapper.Map<List<ActivityResponse>>(activities);

            var listResponse = new ListDataResponse
            {
                TotalRecord = totalRecords,
                TotalPage = totalPages,
                PageIndex = pageIndex,
                Data = activityResponses
            };

            return listResponse;
        }

        public async Task<ActivityResponse> UpdateActivityAsync(Guid userId, Guid activityId, ActivityRequest activityRequest)
        {
            var existingActivity = await activityRepository.GetByIdAsync(activityId, a => a.WalletCategory);
            if (existingActivity == null || existingActivity.WalletCategory.UserId != userId || existingActivity.WalletCategoryId != activityRequest.WalletCategoryId)
            {
                return null;
            }

            existingActivity.Name =  activityRequest.Name;
            existingActivity.Description = activityRequest.Description;

            existingActivity = await activityRepository.UpdateAsync(existingActivity);

            return mapper.Map<ActivityResponse>(existingActivity);
        }
    }
}
