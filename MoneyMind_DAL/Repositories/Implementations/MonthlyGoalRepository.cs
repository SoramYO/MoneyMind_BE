using MoneyMind_DAL.DBContexts;
using MoneyMind_DAL.Entities;
using MoneyMind_DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_DAL.Repositories.Implementations
{
    public class MonthlyGoalRepository : GenericRepository<MonthlyGoal>, IMonthlyGoalRepository
    {
        public MonthlyGoalRepository(MoneyMindDbContext context) : base(context)
        {
        }
    }
}
