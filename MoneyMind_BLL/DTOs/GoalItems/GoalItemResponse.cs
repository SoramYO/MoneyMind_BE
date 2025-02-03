using MoneyMind_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_BLL.DTOs.GoalItems
{
    public class GoalItemResponse
    {
        public Guid Id { get; set; }
        public string Description { get; set; } = string.Empty;

        public double UsedAmount { get; set; }
        public double UsedPercentage { get; set; }

        public double? MinTargetPercentage { get; set; }
        public double? MaxTargetPercentage { get; set; }
        public double? MinAmount { get; set; }
        public double? MaxAmount { get; set; }

        public TargetMode TargetMode { get; set; }
        public bool IsAchieved { get; set; }

        public Guid MonthlyGoalId { get; set; }
        public string MonthlyGoalName { get; set; } = string.Empty;

        public Guid WalletTypeId { get; set; }
        public string WalletTypeName { get; set; } = string.Empty;
    }
}
