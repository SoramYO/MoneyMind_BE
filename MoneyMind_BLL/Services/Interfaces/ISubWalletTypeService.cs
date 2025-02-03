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
    public interface ISubWalletTypeService
    {
        Task CreateDefaultSubWalletTypesAsync(Guid necessitiesId, Guid financialFreedomId, Guid educationId, Guid leisureId, Guid charityId, Guid savingsId, Guid userId);
        Task<ListDataResponse> GetSubWalletTypesAsync(
            Expression<Func<SubWalletType, bool>>? filter,
            Func<IQueryable<SubWalletType>, IOrderedQueryable<SubWalletType>> orderBy,
            string includeProperties,
            int pageIndex,
            int pageSize);
        Task<SubWalletTypeResponse> AddSubWalletTypeAsync(Guid userId, SubWalletTypeRequest subWalletTypeRequest);
        Task<SubWalletTypeResponse> UpdateSubWalletTypeAsync(Guid subWalletTypeId, Guid userId, SubWalletTypeRequest subWalletTypeRequest);
        Task<SubWalletTypeResponse> DeleteSubWalletTypeAsync(Guid subWalletTypeId, Guid userId);
        Task<SubWalletTypeResponse> GetSubWalletTypeByIdAsync(Guid subWalletTypeId);
    }
}
