using AutoMapper;
using MoneyMind_BLL.DTOs;
using MoneyMind_BLL.DTOs.GoalItems;
using MoneyMind_BLL.DTOs.SubWalletTypes;
using MoneyMind_BLL.Services.Interfaces;
using MoneyMind_DAL.Entities;
using MoneyMind_DAL.Repositories.Implementations;
using MoneyMind_DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_BLL.Services.Implementations
{
    public class GoalItemService : IGoalItemService
    {
        private readonly IGoalItemRepository goalItemRepository;
        private readonly IMapper mapper;

        public GoalItemService(IGoalItemRepository goalItemRepository, IMapper mapper)
        {
            this.goalItemRepository = goalItemRepository;
            this.mapper = mapper;
        }
        public async Task<GoalItemResponse> AddGoalItemAsync(GoalItemRequest goalItemRequest)
        {
            // Map or Convert DTO to Domain Model
            var goalItemDomain = mapper.Map<GoalItem>(goalItemRequest);
            // Use Domain Model to create Author
            goalItemDomain = await goalItemRepository.InsertAsync(goalItemDomain);

            return mapper.Map<GoalItemResponse>(goalItemDomain);
        }

        public async Task<ListDataResponse> GetGoalItemAsync(Expression<Func<GoalItem, bool>>? filter, Func<IQueryable<GoalItem>, IOrderedQueryable<GoalItem>> orderBy, string includeProperties, int pageIndex, int pageSize)
        {
            var response = await goalItemRepository.GetAsync(
                        filter: filter,
                        orderBy: orderBy,
                        includeProperties: includeProperties,
                        pageIndex: pageIndex,
                        pageSize: pageSize
                        );
            var subWalletTypes = response.Item1;
            var totalPages = response.Item2;
            var totalRecords = response.Item3;

            // Giả sử authorDomains là danh sách các đối tượng AuthorDomain
            var subWalletTypeResponse = mapper.Map<List<SubWalletTypeResponse>>(subWalletTypes);

            var listResponse = new ListDataResponse
            {
                TotalRecord = totalRecords,
                TotalPage = totalPages,
                PageIndex = pageIndex,
                Data = subWalletTypeResponse
            };

            return listResponse;
        }

        public async Task<GoalItemResponse> GetGoalItemByIdAsync(Guid goalItemId)
        {
            var goalItem = await goalItemRepository.GetByIdAsync(goalItemId, s => s.MonthlyGoal);

            if (goalItem == null)
            {
                return null;
            }

            return mapper.Map<GoalItemResponse>(goalItem);
        }

        public async Task<GoalItemResponse> UpdateGoalItemAsync(Guid goalItemId, Guid userId, GoalItemRequest goalItemRequest)
        {
            var existingGoalItem = await goalItemRepository.GetByIdAsync(goalItemId);
            if (existingGoalItem == null)
            {
                return null;
            }

            existingGoalItem.Description = goalItemRequest.Description;
            existingGoalItem.UsedAmount = goalItemRequest.UsedAmount;
            existingGoalItem.UsedPercentage = goalItemRequest.UsedPercentage;
            existingGoalItem.MinTargetPercentage = goalItemRequest.MinTargetPercentage;
            existingGoalItem.MaxTargetPercentage = goalItemRequest.MaxTargetPercentage;
            existingGoalItem.MinAmount = goalItemRequest.MinAmount;
            existingGoalItem.MaxAmount = goalItemRequest.MaxAmount;
            existingGoalItem.TargetMode = goalItemRequest.TargetMode;
            existingGoalItem.IsAchieved = goalItemRequest.IsAchieved;
            existingGoalItem.MonthlyGoalId = goalItemRequest.MonthlyGoalId;
            existingGoalItem.WalletTypeId = goalItemRequest.WalletTypeId;

            existingGoalItem = await goalItemRepository.UpdateAsync(existingGoalItem);

            return mapper.Map<GoalItemResponse>(existingGoalItem);
        }
    }
}
