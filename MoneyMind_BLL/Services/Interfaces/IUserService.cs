using Microsoft.AspNetCore.Identity;
using MoneyMind_BLL.DTOs.Users;
using MoneyMind_DAL.Entities;

namespace MoneyMind_BLL.Services.Interfaces
{
    public interface IUserService
    {
        Task<double> GetUserBalanceAsync(Guid userId);
        Task<ApplicationUser> GetUserProfileAsync(Guid userId);
        Task<ApplicationUser> UpdateUserProfileAsync(Guid userId, UserProfileRequest request);
        Task<Dictionary<string, double>> GetTransactionsByCategoryAsync(Guid userId, DateTime? fromDate, DateTime? toDate);
    }
} 