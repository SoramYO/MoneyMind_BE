using MoneyMind_BLL.DTOs.GoalItems;
using MoneyMind_BLL.DTOs.MonthlyGoals;
using MoneyMind_BLL.DTOs.WalletCategories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_BLL.DTOs.DataDefaults
{
    public class DataDefaults
    {
        public List<WalletCategoryRequest> WalletCategories { get; set; }
        public MonthlyGoalDefault MonthlyGoal { get; set; }
        public GoalItemDefault GoalItem { get; set; }
    }
}
