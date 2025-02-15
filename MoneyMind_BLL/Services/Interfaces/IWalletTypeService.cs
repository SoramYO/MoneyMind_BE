using MoneyMind_BLL.DTOs;
using MoneyMind_BLL.DTOs.WalletCategories;
using MoneyMind_BLL.DTOs.Wallets;
using MoneyMind_BLL.DTOs.WalletTypes;
using MoneyMind_DAL.Entities;
using MoneyMind_DAL.Repositories.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_BLL.Services.Interfaces
{
    public interface IWalletTypeService
    {
        Task<ListDataResponse> GetWalletTypesAsync(
            Expression<Func<WalletType, bool>>? filter,
            Func<IQueryable<WalletType>, IOrderedQueryable<WalletType>> orderBy,
            string includeProperties,
            int pageIndex,
            int pageSize);
        Task<WalletTypeResponse> AddWalletTypeAsync(WalletTypeRequest walletTypeRequest);
        Task<WalletTypeResponse> UpdateWalletTypeAsync(Guid walletTypeId, WalletTypeRequest walletTypeRequest);
        Task<WalletTypeResponse> DeleteWalletTypeAsync(Guid walletTypeId);
        Task<WalletTypeResponse> GetWalletTypeByIdAsync(Guid walletTypeId);
    }
}
