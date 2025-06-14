﻿using MoneyMind_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_BLL.DTOs.GoalItems
{
    public class GoalItemRequest
    {
        public string Description { get; set; } = string.Empty;
        public double? MinTargetPercentage { get; set; }
        public double? MaxTargetPercentage { get; set; }
        public double? MinAmount { get; set; }
        public double? MaxAmount { get; set; }
        public TargetMode TargetMode { get; set; } = TargetMode.NoTarget;

        /// <summary>
        /// ID của MonthlyGoal mà GoalItem thuộc về
        /// </summary>
        public Guid MonthlyGoalId { get; set; }

        /// <summary>
        /// ID của loại ví (WalletType)
        /// </summary>
        public Guid WalletTypeId { get; set; }
    }
}
