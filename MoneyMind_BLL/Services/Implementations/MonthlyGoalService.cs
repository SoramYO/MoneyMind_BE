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
        private readonly IMonthlyGoalRepository monthlyGoalRepository;
        private readonly ITransactionRepository transactionRepository;
        private readonly IGoalItemRepository goalItemRepository;
        private readonly IMapper mapper;

        public MonthlyGoalService(IMonthlyGoalRepository monthlyGoalRepository,
            ITransactionRepository transactionRepository,
            IGoalItemRepository goalItemRepository,
            IMapper mapper)
        {
            this.monthlyGoalRepository = monthlyGoalRepository;
            this.transactionRepository = transactionRepository;
            this.goalItemRepository = goalItemRepository;
            this.mapper = mapper;
        }
        public async Task<MonthlyGoalResponse> AddMonthlyGoalAsync(Guid userId, MonthlyGoalRequest monthlyGoalRequest)
        {
            // Map or Convert DTO to Domain Model
            var monthlyGoalDomain = mapper.Map<MonthlyGoal>(monthlyGoalRequest);
            monthlyGoalDomain.UserId = userId;
            // Use Domain Model to create Author

            if (monthlyGoalRequest.GoalItems != null && monthlyGoalRequest.GoalItems.Any())
            {
                var goalItems = mapper.Map<List<GoalItem>>(monthlyGoalRequest.GoalItems);
                foreach (var goalItem in goalItems)
                {
                    goalItem.MonthlyGoalId = monthlyGoalDomain.Id; // Gán MonthlyGoalId cho từng GoalItem

                    // Tính UsedAmount từ các giao dịch trước đó
                    goalItem.UsedAmount = await transactionRepository.GetSumAsync(
                        t => t.UserId == userId
                          && t.Wallet != null
                          && t.Wallet.SubWalletType != null
                          && t.Wallet.SubWalletType.WalletTypeId == goalItem.WalletTypeId
                          && t.TransactionDate.Month == monthlyGoalDomain.Month
                          && t.TransactionDate.Year == monthlyGoalDomain.Year,
                        t => t.Amount
                    );

                    // Tính phần trăm sử dụng
                    goalItem.UsedPercentage = (goalItem.UsedAmount / monthlyGoalDomain.TotalAmount) * 100;
                }
                monthlyGoalDomain.GoalItems = goalItems;
            }
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


        public async Task<MonthlyGoalResponse> UpdateMonthlyGoalAsync(Guid monthlyGoalId, Guid userId, MonthlyGoalRequest monthlyGoalRequest)
        {
            var existingMonthlyGoal = await monthlyGoalRepository.GetByIdAsync(monthlyGoalId, s => s.GoalItems);

            if (existingMonthlyGoal == null || existingMonthlyGoal.UserId != userId)
            {
                return null;
            }

            // Cập nhật thông tin MonthlyGoal
            existingMonthlyGoal.TotalAmount = monthlyGoalRequest.TotalAmount;
            existingMonthlyGoal.Month = monthlyGoalRequest.Month;
            existingMonthlyGoal.Year = monthlyGoalRequest.Year;
            existingMonthlyGoal.Status = monthlyGoalRequest.Status;
            existingMonthlyGoal.IsCompleted = monthlyGoalRequest.IsCompleted;

            // Lấy danh sách GoalItem hiện tại
            var existingGoalItems = existingMonthlyGoal.GoalItems.ToList();

            // Nếu có danh sách GoalItem mới
            if (monthlyGoalRequest.GoalItems != null)
            {
                var newGoalItems = mapper.Map<List<GoalItem>>(monthlyGoalRequest.GoalItems);

                // Lấy danh sách GoalItem bị xóa
                var deletedGoalItems = existingGoalItems.Where(e => !newGoalItems.Any(n => n.Id == e.Id)).ToList();

                // Xóa các GoalItem bị loại bỏ
                if (deletedGoalItems.Any())
                {
                    foreach (var deletedGoalItem in deletedGoalItems)
                    {
                        await goalItemRepository.DeleteAsync(deletedGoalItem);
                    }
                }

                // Xử lý các GoalItem mới hoặc cập nhật
                foreach (var newGoalItem in newGoalItems)
                {
                    newGoalItem.MonthlyGoalId = existingMonthlyGoal.Id;

                    // Nếu GoalItem đã tồn tại, cập nhật thông tin
                    var existingGoalItem = existingGoalItems.FirstOrDefault(e => e.Id == newGoalItem.Id);
                    if (existingGoalItem != null)
                    {
                        existingGoalItem.Description = newGoalItem.Description;
                        existingGoalItem.MinTargetPercentage = newGoalItem.MinTargetPercentage;
                        existingGoalItem.MaxTargetPercentage = newGoalItem.MaxTargetPercentage;
                        existingGoalItem.MinAmount = newGoalItem.MinAmount;
                        existingGoalItem.MaxAmount = newGoalItem.MaxAmount;
                        existingGoalItem.TargetMode = newGoalItem.TargetMode;
                        existingGoalItem.IsAchieved = newGoalItem.IsAchieved;
                        existingGoalItem.WalletTypeId = newGoalItem.WalletTypeId;

                        // Nếu loại ví thay đổi, tính lại UsedAmount
                        if (existingGoalItem.WalletTypeId != newGoalItem.WalletTypeId)
                        {
                            existingGoalItem.UsedAmount = await transactionRepository.GetSumAsync(
                                t => t.UserId == userId
                                  && t.Wallet.SubWalletType.WalletTypeId == newGoalItem.WalletTypeId
                                  && t.TransactionDate.Month == existingMonthlyGoal.Month
                                  && t.TransactionDate.Year == existingMonthlyGoal.Year,
                                t => t.Amount
                            );
                        }

                        existingGoalItem.UsedPercentage = (existingGoalItem.UsedAmount / existingMonthlyGoal.TotalAmount) * 100;
                        await goalItemRepository.UpdateAsync(existingGoalItem);
                    }
                    else
                    {
                        // Thêm GoalItem mới
                        newGoalItem.UsedAmount = await transactionRepository.GetSumAsync(
                            t => t.UserId == userId
                              && t.Wallet.SubWalletType.WalletTypeId == newGoalItem.WalletTypeId
                              && t.TransactionDate.Month == existingMonthlyGoal.Month
                              && t.TransactionDate.Year == existingMonthlyGoal.Year,
                            t => t.Amount
                        );

                        newGoalItem.UsedPercentage = (newGoalItem.UsedAmount / existingMonthlyGoal.TotalAmount) * 100;
                        await goalItemRepository.InsertAsync(newGoalItem);
                    }
                }
            }

            // Cập nhật MonthlyGoal
            existingMonthlyGoal = await monthlyGoalRepository.UpdateAsync(existingMonthlyGoal);

            return mapper.Map<MonthlyGoalResponse>(existingMonthlyGoal);
        }

    }
}
