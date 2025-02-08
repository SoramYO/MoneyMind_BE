using Microsoft.AspNetCore.Identity;
using MoneyMind_BLL.DTOs.Users;

namespace MoneyMind_BLL.Services.Interfaces
{
    public interface IUserService
    {
        Task<double> GetUserBalanceAsync(Guid userId);
        Task<IdentityUser> GetUserProfileAsync(Guid userId);
        Task<IdentityUser> UpdateUserProfileAsync(Guid userId, UserProfileRequest request);
        Task<Dictionary<string, double>> GetTransactionsByCategoryAsync(Guid userId, DateTime? fromDate, DateTime? toDate);
    }
} 