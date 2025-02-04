
using MoneyMind_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_DAL.Repositories.Interfaces
{
    public interface IGoalItemRepository : IGenericRepository<GoalItem>
    {
        Task<GoalItem> GetByWalletTypeAsync(Guid userId, Guid walletTypeId, Guid monthlyGoalId);
    }
}
