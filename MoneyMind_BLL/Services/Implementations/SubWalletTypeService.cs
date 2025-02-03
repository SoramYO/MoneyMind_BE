using AutoMapper;
using MoneyMind_BLL.DTOs;
using MoneyMind_BLL.DTOs.SubWalletTypes;
using MoneyMind_BLL.Services.Interfaces;
using MoneyMind_DAL.Entities;
using MoneyMind_DAL.Repositories.Interfaces;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using MoneyMind_BLL.DTOs.MonthlyGoals;
using MoneyMind_DAL.Repositories.Implementations;

namespace MoneyMind_BLL.Services.Implementations
{
    public class SubWalletTypeService : ISubWalletTypeService
    {
        private readonly ISubWalletTypeRepository subWalletTypeRepository;
        private readonly IMapper mapper;
        private readonly IHttpContextAccessor httpContextAccessor;

        public SubWalletTypeService(ISubWalletTypeRepository subWalletTypeRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            this.subWalletTypeRepository = subWalletTypeRepository;
            this.mapper = mapper;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<SubWalletTypeResponse> AddSubWalletTypeAsync(Guid userId, SubWalletTypeRequest subWalletTypeRequest)
        {

            // Map or Convert DTO to Domain Model
            var subWalletTypeDomain = mapper.Map<SubWalletType>(subWalletTypeRequest);
            subWalletTypeDomain.UserId = userId;
            // Use Domain Model to create Author
            subWalletTypeDomain = await subWalletTypeRepository.InsertAsync(subWalletTypeDomain);

            return mapper.Map<SubWalletTypeResponse>(subWalletTypeDomain);
        }

        public async Task CreateDefaultSubWalletTypesAsync(Guid necessitiesId, Guid financialFreedomId, Guid educationId, Guid leisureId, Guid charityId, Guid savingsId, Guid userId)
        {
            var request = httpContextAccessor.HttpContext?.Request;
            var baseUrl = $"{request.Scheme}://{request.Host.Value}";

            var defaultSubWalletTypes = new List<(string Name, string Description, Guid WalletTypeId, string IconName, string Color)>
            {
                // Necessities
                ("Housing", "Expenses for rent or mortgage.", necessitiesId, "housing.png", "#FF5733"),
                ("Transportation", "Expenses for fuel or public transport.", necessitiesId, "transportation.png", "#33C1FF"),
                ("Food", "Daily groceries or meals.", necessitiesId, "food.png", "#33FF57"),
                ("Utilities", "Electricity, water, and internet bills.", necessitiesId, "utilities.png", "#FFC133"),

                // Financial Freedom
                ("Investments", "Long-term wealth building.", financialFreedomId, "investments.png", "#5733FF"),
                ("Savings for Freedom", "Funds for achieving financial independence.", financialFreedomId, "savings_freedom.png", "#FF33C1"),

                // Education
                ("Tuition", "Fees for schools or universities.", educationId, "tuition.png", "#3388FF"),
                ("Learning Materials", "Books, courses, or online materials.", educationId, "learning_materials.png", "#88FF33"),

                // Leisure
                ("Entertainment", "Movies, games, or music subscriptions.", leisureId, "entertainment.png", "#FF3388"),
                ("Travel", "Vacation or trips.", leisureId, "travel.png", "#FF5733"),
                ("Dining Out", "Meals at restaurants or cafes.", leisureId, "dining_out.png", "#FFC133"),

                // Charity
                ("Donations", "Funds donated to charities or causes.", charityId, "donations.png", "#5733FF"),

                // Savings
                ("Emergency Fund", "Savings for unexpected events.", savingsId, "emergency_fund.png", "#FF5733"),
            };

            var subWalletTypes = defaultSubWalletTypes.Select(item => CreateSubWalletType(
                name: item.Name,
                description: item.Description,
                walletTypeId: item.WalletTypeId,
                iconPath: $"{baseUrl}/Assets/Icons/{item.IconName}",
                color: item.Color,
                userId: userId
            )).ToList();

            foreach (var subWalletType in subWalletTypes)
            {
                await subWalletTypeRepository.InsertAsync(subWalletType);
            }
        }


        public async Task<SubWalletTypeResponse> DeleteSubWalletTypeAsync(Guid subWalletTypeId, Guid userId)
        {
            var existingSubWalletType = await subWalletTypeRepository.GetByIdAsync(subWalletTypeId);
            if (existingSubWalletType == null || existingSubWalletType.UserId != userId)
            {
                return null;
            }

            existingSubWalletType.IsActive = false;

            existingSubWalletType = await subWalletTypeRepository.UpdateAsync(existingSubWalletType);

            return mapper.Map<SubWalletTypeResponse>(existingSubWalletType);
        }

        public async Task<SubWalletTypeResponse> GetSubWalletTypeByIdAsync(Guid subWalletTypeId)
        {
            var subWallet = await subWalletTypeRepository.GetByIdAsync(subWalletTypeId, s => s.WalletType);

            if (subWallet == null)
            {
                return null;
            }

            return mapper.Map<SubWalletTypeResponse>(subWallet);
        }

        public async Task<ListDataResponse> GetSubWalletTypesAsync(
            Expression<Func<SubWalletType, bool>>? filter,
            Func<IQueryable<SubWalletType>, IOrderedQueryable<SubWalletType>> orderBy,
            string includeProperties,
            int pageIndex,
            int pageSize)
        {
            var response = await subWalletTypeRepository.GetAsync(
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

        public async Task<SubWalletTypeResponse> UpdateSubWalletTypeAsync(Guid subWalletTypeId, Guid userId, SubWalletTypeRequest subWalletTypeRequest)
        {
            var existingSubWalletType = await subWalletTypeRepository.GetByIdAsync(subWalletTypeId);
            if (existingSubWalletType == null || existingSubWalletType.UserId != userId)
            {
                return null;
            }

            existingSubWalletType.Name = subWalletTypeRequest.Name;
            existingSubWalletType.Description = subWalletTypeRequest.Description;
            existingSubWalletType.IconPath = subWalletTypeRequest.IconPath;
            existingSubWalletType.Color = subWalletTypeRequest.Color;
            existingSubWalletType.WalletTypeId = subWalletTypeRequest.WalletTypeId;

            existingSubWalletType = await subWalletTypeRepository.UpdateAsync(existingSubWalletType);

            return mapper.Map<SubWalletTypeResponse>(existingSubWalletType);
        }

        private SubWalletType CreateSubWalletType(string name, string description, Guid walletTypeId, string iconPath, string color, Guid userId)
        {
            return new SubWalletType
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = description,
                WalletTypeId = walletTypeId,
                IconPath = iconPath,
                Color = color,
                UserId = userId
            };
        }
    }
}
