using MoneyMind_BLL.DTOs.Users;

namespace MoneyMind_BLL.Services.Interfaces
{
    public interface IUserService
    {
        Task<double> GetUserBalanceAsync(Guid userId);
        Task<UserProfileResponse> GetUserProfileAsync(Guid userId);
        Task<UserProfileResponse> UpdateUserProfileAsync(Guid userId, UserProfileRequest request);
        Task<Dictionary<string, double>> GetTransactionsByCategoryAsync(Guid userId, DateTime? fromDate, DateTime? toDate);
    }
} 