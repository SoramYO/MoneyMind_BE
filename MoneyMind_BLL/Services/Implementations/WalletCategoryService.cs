using AutoMapper;
using MoneyMind_BLL.DTOs;
using MoneyMind_BLL.DTOs.WalletCategories;
using MoneyMind_BLL.Services.Interfaces;
using MoneyMind_DAL.Entities;
using MoneyMind_DAL.Repositories.Interfaces;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using MoneyMind_BLL.DTOs.MonthlyGoals;
using MoneyMind_DAL.Repositories.Implementations;
using System.Text.Json;
using MoneyMind_BLL.DTOs.DataDefaults;

namespace MoneyMind_BLL.Services.Implementations
{
    public class WalletCategoryService : IWalletCategoryService
    {
        private readonly IWalletCategoryRepository walletCategoryRepository;
        private readonly IMapper mapper;

        public WalletCategoryService(IWalletCategoryRepository walletCategoryRepository,
            IMapper mapper)
        {
            this.walletCategoryRepository = walletCategoryRepository;
            this.mapper = mapper;
        }

        public async Task<WalletCategoryResponse> AddWalletCategoryAsync(Guid userId, WalletCategoryRequest walletCategoryRequest)
        {

            // Map or Convert DTO to Domain Model
            var walletCategoryDomain = mapper.Map<WalletCategory>(walletCategoryRequest);
            walletCategoryDomain.UserId = userId;
            // Use Domain Model to create Author
            walletCategoryDomain = await walletCategoryRepository.InsertAsync(walletCategoryDomain);

            return mapper.Map<WalletCategoryResponse>(walletCategoryDomain);
        }

        public async Task<IEnumerable<WalletCategoryResponse>> CreateDefaultWalletCategoriesAndActivitiesAsync(Guid userId)
        {
            var jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "datadefaults.json");
            var jsonString = await File.ReadAllTextAsync(jsonFilePath);
            var dataDefaults = JsonSerializer.Deserialize<DataDefaults>(jsonString);

            var defaultWalletCategories = dataDefaults.WalletCategories;

            var responses = new List<WalletCategoryResponse>();

            foreach (var request in defaultWalletCategories)
            {
                // Map DTO sang Domain Model
                var walletCategoryDomain = mapper.Map<WalletCategory>(request);
                walletCategoryDomain.UserId = userId;

                // Chèn vào DB
                walletCategoryDomain = await walletCategoryRepository.InsertAsync(walletCategoryDomain);

                // Map Domain Model sang Response
                responses.Add(mapper.Map<WalletCategoryResponse>(walletCategoryDomain));
            }

            return responses;
        }

        public async Task<WalletCategoryResponse> DeleteWalletCategoryAsync(Guid walletCategoryId, Guid userId)
        {
            var existingWalletCategory = await walletCategoryRepository.GetByIdAsync(walletCategoryId);
            if (existingWalletCategory == null || existingWalletCategory.UserId != userId)
            {
                return null;
            }

            existingWalletCategory.IsActive = false;

            existingWalletCategory = await walletCategoryRepository.UpdateAsync(existingWalletCategory);

            return mapper.Map<WalletCategoryResponse>(existingWalletCategory);
        }

        public async Task<WalletCategoryResponse> GetWalletCategoryByIdAsync(Guid walletCategoryId)
        {
            var walletCategory = await walletCategoryRepository.GetByIdAsync(walletCategoryId, s => s.WalletType, s => s.Activities);

            if (walletCategory == null)
            {
                return null;
            }

            walletCategory.Activities = walletCategory.Activities.Where(a => !a.IsDeleted).ToList();

            return mapper.Map<WalletCategoryResponse>(walletCategory);
        }

        public async Task<ListDataResponse> GetWalletCategoriesAsync(
            Expression<Func<WalletCategory, bool>>? filter,
            Func<IQueryable<WalletCategory>, IOrderedQueryable<WalletCategory>> orderBy,
            string includeProperties,
            int pageIndex,
            int pageSize)
        {
            var response = await walletCategoryRepository.GetAsync(
                        filter: filter,
                        orderBy: orderBy,
                        includeProperties: includeProperties,
                        pageIndex: pageIndex,
                        pageSize: pageSize
                        );
            var walletCategories = response.Item1;
            var totalPages = response.Item2;
            var totalRecords = response.Item3;

            foreach (var walletCategory in walletCategories)
            {
                walletCategory.Activities = walletCategory.Activities.Where(a => !a.IsDeleted).ToList();
            }

            // Giả sử authorDomains là danh sách các đối tượng AuthorDomain
            var walletCategoryResponses = mapper.Map<List<WalletCategoryResponse>>(walletCategories);



            var listResponse = new ListDataResponse
            {
                TotalRecord = totalRecords,
                TotalPage = totalPages,
                PageIndex = pageIndex,
                Data = walletCategoryResponses
            };

            return listResponse;
        }

        public async Task<WalletCategoryResponse> UpdateWalletCategoryAsync(Guid walletCategoryId, Guid userId, WalletCategoryRequest walletCategoryRequest)
        {
            var existingWalletCategory = await walletCategoryRepository.GetByIdAsync(walletCategoryId, s => s.Activities);
            if (existingWalletCategory == null || existingWalletCategory.UserId != userId || existingWalletCategory.WalletTypeId == walletCategoryRequest.WalletTypeId)
            {
                return null;
            }

            existingWalletCategory.Name = walletCategoryRequest.Name;
            existingWalletCategory.Description = walletCategoryRequest.Description;
            existingWalletCategory.IconPath = walletCategoryRequest.IconPath;
            existingWalletCategory.Color = walletCategoryRequest.Color;

            existingWalletCategory = await walletCategoryRepository.UpdateAsync(existingWalletCategory);

            return mapper.Map<WalletCategoryResponse>(existingWalletCategory);
        }
    }
}
