using AutoMapper;
using Microsoft.AspNetCore.Identity;
using MoneyMind_BLL.DTOs;
using MoneyMind_BLL.DTOs.Admin;
using MoneyMind_BLL.Services.Interfaces;
using MoneyMind_DAL.Entities;
using MoneyMind_DAL.Repositories.Interfaces;
using System.Linq.Expressions;

namespace MoneyMind_BLL.Services.Implementations
{
    public class AdminService : IAdminService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMapper _mapper;

        public AdminService(
            UserManager<IdentityUser> userManager,
            ITransactionRepository transactionRepository,
            IMapper mapper)
        {
            _userManager = userManager;
            _transactionRepository = transactionRepository;
            _mapper = mapper;
        }

        public async Task<ListDataResponse> GetAllUsersAsync(int pageIndex, int pageSize)
        {
            var users = _userManager.Users
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var totalUsers = _userManager.Users.Count();
            var totalPages = (int)Math.Ceiling(totalUsers / (double)pageSize);

            return new ListDataResponse
            {
                Data = users,
                PageIndex = pageIndex,
                TotalPage = totalPages,
                TotalRecord = totalUsers
            };
        }

        public async Task<IdentityUser> CreateUserAsync(AdminCreateUserRequest request)
        {
            var user = new IdentityUser
            {
                UserName = request.Email,
                Email = request.Email
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                throw new Exception($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            await _userManager.AddToRoleAsync(user, request.Role);
            return user;
        }

        public async Task<IdentityUser> UpdateUserAsync(string userId, AdminUpdateUserRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            if (!string.IsNullOrEmpty(request.Email))
            {
                user.Email = request.Email;
                user.UserName = request.Email;
            }

            if (!string.IsNullOrEmpty(request.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, request.Password);
                if (!result.Succeeded)
                {
                    throw new Exception($"Failed to update password: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }

            if (!string.IsNullOrEmpty(request.Role))
            {
                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRoleAsync(user, request.Role);
            }

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                throw new Exception($"Failed to update user: {string.Join(", ", updateResult.Errors.Select(e => e.Description))}");
            }

            return user;
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

        public async Task<AdminReportResponse> GetReportsAsync()
        {
            var response = new AdminReportResponse
            {
                TotalUsers = _userManager.Users.Count(),
                TotalTransactions = (await _transactionRepository.GetAsync()).Item1.Count()
            };

            var transactions = (await _transactionRepository.GetAsync(includeProperties: "Tag")).Item1;
            var expensesByCategory = transactions
                .GroupBy(t => t.Tag.Name)
                .ToDictionary(g => g.Key, g => g.Sum(t => t.Amount));

            response.ExpensesByCategory = expensesByCategory;
            return response;
        }

        public async Task<ListDataResponse> GetAllTransactionsAsync(
            Expression<Func<Transaction, bool>>? filter,
            Func<IQueryable<Transaction>, IOrderedQueryable<Transaction>> orderBy,
            string includeProperties,
            int pageIndex,
            int pageSize)
        {
            var result = await _transactionRepository.GetAsync(
                filter: filter,
                orderBy: orderBy,
                includeProperties: includeProperties,
                pageIndex: pageIndex,
                pageSize: pageSize);

            var transactions = result.Item1;
            var transactionDtos = transactions.Select(t => new
            {
                t.Id,
                t.Amount,
                t.Description,
                t.TransactionDate,
                t.RecipientName,
                t.IsActive,
                t.CreateAt,
                t.UpdatedAt,
                Tags = new
                {
                    t.Tag.Id,
                    t.Tag.Name,
                    t.Tag.Description,
                    t.Tag.Color
                }
            }).ToList();

            return new ListDataResponse
            {
                Data = transactionDtos,
                PageIndex = pageIndex,
                TotalPage = result.Item2,
                TotalRecord = result.Item3
            };
        }
    }
} 