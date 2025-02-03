using MoneyMind_BLL.DTOs;
using MoneyMind_BLL.DTOs.SubWalletTypes;
using MoneyMind_DAL.Entities;
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
    }
}
