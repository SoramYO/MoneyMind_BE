using MoneyMind_BLL.DTOs;
using MoneyMind_BLL.DTOs.WalletCategories;
using MoneyMind_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_BLL.Services.Interfaces
{
    public interface IWalletCategoryService
    {
        //Task CreateDefaultSubWalletTypesAndActivitiesAsync(Guid necessitiesId, Guid financialFreedomId, Guid educationId, Guid leisureId, Guid charityId, Guid savingsId, Guid userId);
        Task<IEnumerable<WalletCategoryResponse>> CreateDefaultWalletCategoriesAndActivitiesAsync(Guid userId);
        Task<ListDataResponse> GetWalletCategoriesAsync(
            Expression<Func<WalletCategory, bool>>? filter,
            Func<IQueryable<WalletCategory>, IOrderedQueryable<WalletCategory>> orderBy,
            string includeProperties,
            int pageIndex,
            int pageSize);
        Task<WalletCategoryResponse> AddWalletCategoryAsync(Guid userId, WalletCategoryRequest walletCategoryRequest);
        Task<WalletCategoryResponse> UpdateWalletCategoryAsync(Guid walletCategoryId, Guid userId, WalletCategoryRequest walletCategoryRequest);
        Task<WalletCategoryResponse> DeleteWalletCategoryAsync(Guid walletCategoryId, Guid userId);
        Task<WalletCategoryResponse> GetWalletCategoryByIdAsync(Guid walletCategoryId);
    }
}
