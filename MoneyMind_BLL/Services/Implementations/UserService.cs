using AutoMapper;
using MoneyMind_BLL.DTOs.Users;
using MoneyMind_BLL.Services.Interfaces;
using MoneyMind_DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace MoneyMind_BLL.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMapper _mapper;

        public UserService(
            UserManager<IdentityUser> userManager,
            ITransactionRepository transactionRepository,
            IMapper mapper)
        {
            _userManager = userManager;
            _transactionRepository = transactionRepository;
            _mapper = mapper;
        }

        public async Task<double> GetUserBalanceAsync(Guid userId)
        {
            var transactions = await _transactionRepository.GetAsync(
                filter: t => t.UserId == userId && t.IsActive);
            
            return transactions.Item1.Sum(t => t.Amount);
        }

        public async Task<UserProfileResponse> GetUserProfileAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new Exception("User not found");

            return _mapper.Map<UserProfileResponse>(user);
        }

        public async Task<UserProfileResponse> UpdateUserProfileAsync(Guid userId, UserProfileRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new Exception("User not found");

            // Update user properties
            _mapper.Map(request, user);
            
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new Exception($"Failed to update user: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            return _mapper.Map<UserProfileResponse>(user);
        }

        public async Task<Dictionary<string, double>> GetTransactionsByCategoryAsync(Guid userId, DateTime? fromDate, DateTime? toDate)
        {
            var transactions = await _transactionRepository.GetAsync(
                filter: t => t.UserId == userId && t.IsActive &&
                            (!fromDate.HasValue || t.TransactionDate >= fromDate) &&
                            (!toDate.HasValue || t.TransactionDate <= toDate),
                includeProperties: "Tag");

            return transactions.Item1
                .GroupBy(t => t.Tag.Name)
                .ToDictionary(g => g.Key, g => g.Sum(t => t.Amount));
        }
    }
} 