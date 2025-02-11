using AutoMapper;
using Microsoft.AspNetCore.Identity;
using MoneyMind_BLL.DTOs;
using MoneyMind_BLL.DTOs.Accounts;
using MoneyMind_BLL.DTOs.Admin;
using MoneyMind_BLL.Services.Interfaces;
using MoneyMind_DAL.Entities;
using MoneyMind_DAL.Repositories.Interfaces;
using System.Linq.Expressions;

namespace MoneyMind_BLL.Services.Implementations
{
    public class AdminService : IAdminService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMapper _mapper;

        public AdminService(
            UserManager<ApplicationUser> userManager,
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

			var userDtos = new List<object>();
			foreach (var user in users)
			{
				var roles = await _userManager.GetRolesAsync(user);
				userDtos.Add(new
				{
					user.Id,
					user.UserName,
					user.Email,
                    user.EmailConfirmed,
					Roles = roles
				});
			}

			return new ListDataResponse
            {
                Data = userDtos,
                PageIndex = pageIndex,
                TotalPage = totalPages,
                TotalRecord = totalUsers
            };
        }

        public async Task<ApplicationUser> CreateUserAsync(AdminCreateUserRequest request)
        {
			var existingUser = await _userManager.FindByEmailAsync(request.Email);
			if (existingUser != null)
			{
				throw new Exception("User with this email already exists");
			}
			
            var user = new ApplicationUser
            {
				UserName = request.Email,
				FullName = request.FullName,
                Email = request.Email
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                throw new Exception($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            var roleResult = await _userManager.AddToRoleAsync(user, request.Role);
			if (!roleResult.Succeeded)
			{
				// If role assignment fails, delete the created user
				await _userManager.DeleteAsync(user);
				throw new Exception($"Failed to assign role: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
			}
			return user;
        }

        public async Task<ApplicationUser> UpdateUserAsync(string userId, AdminUpdateUserRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            if (!string.IsNullOrEmpty(request.Email))
            {
                user.UserName = request.UserName;
                user.Email = request.Email;
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

			user.EmailConfirmed = false;

			var result = await _userManager.UpdateAsync(user);
			if (!result.Succeeded)
			{
				throw new Exception($"Failed to disable user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
			}
			return result.Succeeded;
        }

        public async Task<AdminReportResponse> GetReportsAsync()
        {
            var response = new AdminReportResponse
            {
                TotalUsers = _userManager.Users.Count(),
                TotalTransactions = (await _transactionRepository.GetAsync()).Item1.Count()
            };

            // Lấy tất cả giao dịch và bao gồm TransactionTags + Tag
            var transactions = (await _transactionRepository.GetAsync(includeProperties: "TransactionTags.Tag")).Item1;

            // Nhóm các giao dịch theo Tag.Name
            var expensesByCategory = transactions
                .SelectMany(t => t.TransactionTags) // Lấy tất cả các TransactionTags của từng Transaction
                .GroupBy(tt => tt.Tag.Name) // Nhóm theo Tag.Name
                .ToDictionary(g => g.Key, g => g.Sum(tt => tt.Transaction.Amount)); // Cộng tổng số tiền từ Transaction

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
                includeProperties: "TransactionTags.Tag", // Load TransactionTags + Tag
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
                Tags = t.TransactionTags.Select(tt => new // Duyệt qua TransactionTags để lấy Tags
                {
                    tt.Tag.Id,
                    tt.Tag.Name,
                    tt.Tag.Description,
                    tt.Tag.Color
                }).ToList()
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