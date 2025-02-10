using Microsoft.AspNetCore.Identity;
using MoneyMind_BLL.DTOs;
using MoneyMind_BLL.DTOs.Admin;
using MoneyMind_BLL.DTOs.Transactions;
using MoneyMind_DAL.Entities;
using System.Linq.Expressions;

namespace MoneyMind_BLL.Services.Interfaces
{
    public interface IAdminService
    {
        Task<ListDataResponse> GetAllUsersAsync(int pageIndex, int pageSize);
        Task<ApplicationUser> CreateUserAsync(AdminCreateUserRequest request);
        Task<ApplicationUser> UpdateUserAsync(string userId, AdminUpdateUserRequest request);
        Task<bool> DeleteUserAsync(string userId);
        Task<AdminReportResponse> GetReportsAsync();
        Task<ListDataResponse> GetAllTransactionsAsync(
            Expression<Func<Transaction, bool>>? filter,
            Func<IQueryable<Transaction>, IOrderedQueryable<Transaction>> orderBy,
            string includeProperties,
            int pageIndex,
            int pageSize);
    }
} 