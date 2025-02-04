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
        private readonly IMonthlyGoalRepository _monthlyGoalRepository;
        private readonly IMapper _mapper;

        public MonthlyGoalService(IMonthlyGoalRepository monthlyGoalRepository, IMapper mapper)
        {
            _monthlyGoalRepository = monthlyGoalRepository;
            _mapper = mapper;
        }

        public async Task<MonthlyGoalResponse> AddMonthlyGoalAsync(Guid userId, MonthlyGoalRequest monthlyGoalRequest)
        {
            var monthlyGoal = _mapper.Map<MonthlyGoal>(monthlyGoalRequest);
            monthlyGoal.UserId = userId;
            monthlyGoal.CreateAt = DateTime.UtcNow;
            monthlyGoal.Status = GoalStatus.InProgress;
            monthlyGoal.IsCompleted = false;

            monthlyGoal = await _monthlyGoalRepository.InsertAsync(monthlyGoal);
            return _mapper.Map<MonthlyGoalResponse>(monthlyGoal);
        }

        public async Task<ListDataResponse> GetMonthlyGoalAsync(
            Expression<Func<MonthlyGoal, bool>>? filter,
            Func<IQueryable<MonthlyGoal>, IOrderedQueryable<MonthlyGoal>> orderBy,
            string includeProperties,
            int pageIndex,
            int pageSize)
        {
            var response = await _monthlyGoalRepository.GetAsync(
                filter: filter,
                orderBy: orderBy,
                includeProperties: includeProperties,
                pageIndex: pageIndex,
                pageSize: pageSize);

            var monthlyGoals = response.Item1;
            var totalPages = response.Item2;
            var totalRecords = response.Item3;

            var monthlyGoalResponses = _mapper.Map<List<MonthlyGoalResponse>>(monthlyGoals);

            return new ListDataResponse
            {
                Data = monthlyGoalResponses,
                PageIndex = pageIndex,
                TotalPage = totalPages,
                TotalRecord = totalRecords
            };
        }

        public async Task<MonthlyGoalResponse> GetMonthlyGoalByIdAsync(Guid monthlyGoalId)
        {
            var monthlyGoal = await _monthlyGoalRepository.GetByIdAsync(monthlyGoalId, g => g.GoalItems);

            if (monthlyGoal == null)
            {
                return null;
            }

            return _mapper.Map<MonthlyGoalResponse>(monthlyGoal);
        }

        public async Task<MonthlyGoalResponse> UpdateMonthlyGoalAsync(Guid monthlyGoalId, Guid userId, MonthlyGoalRequest monthlyGoalRequest)
        {
            var existingMonthlyGoal = await _monthlyGoalRepository.GetByIdAsync(monthlyGoalId);

            if (existingMonthlyGoal == null || existingMonthlyGoal.UserId != userId)
            {
                return null;
            }

            existingMonthlyGoal.TotalAmount = monthlyGoalRequest.TotalAmount;
            existingMonthlyGoal.Month = monthlyGoalRequest.Month;
            existingMonthlyGoal.Year = monthlyGoalRequest.Year;
            existingMonthlyGoal.Status = monthlyGoalRequest.Status;
            existingMonthlyGoal.IsCompleted = monthlyGoalRequest.IsCompleted;

            existingMonthlyGoal = await _monthlyGoalRepository.UpdateAsync(existingMonthlyGoal);

            return _mapper.Map<MonthlyGoalResponse>(existingMonthlyGoal);
        }
    }
}
