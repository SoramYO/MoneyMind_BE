using MoneyMind_BLL.DTOs.SubWalletTypes;
using MoneyMind_BLL.DTOs;
using MoneyMind_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MoneyMind_BLL.DTOs.Wallets;

namespace MoneyMind_BLL.Services.Interfaces
{
    public interface IWalletService
    {
        Task<ListDataResponse> GetWalletsAsync(
            Expression<Func<Wallet, bool>>? filter,
            Func<IQueryable<Wallet>, IOrderedQueryable<Wallet>> orderBy,
            string includeProperties,
            int pageIndex,
            int pageSize);
        Task<WalletResponse> AddWalletAsync(Guid userId, WalletRequest walletRequest);
        Task<WalletResponse> UpdateWalletAsync(Guid walletId, Guid userId, WalletRequest walletRequest);
        Task<WalletResponse> DeleteWalletAsync(Guid walletId, Guid userId);
        Task<WalletResponse> GetWalletByIdAsync(Guid walletId);
    }
}
