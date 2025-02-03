using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_DAL.Entities
{
    public enum TargetMode
    {
        MaxOnly,          // Chỉ kiểm tra giá trị tối đa.
        MinOnly,          // Chỉ kiểm tra giá trị tối thiểu.
        Range,            // Kiểm tra cả giá trị tối thiểu và tối đa (theo khoảng).
        PercentageOnly,   // Chỉ kiểm tra tỷ lệ phần trăm.
        FixedAmount,      // Giá trị cố định.
        NoTarget          // Không có mục tiêu cụ thể.
    }
    public class GoalItem
    {
        public GoalItem()
        {
            Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }
        public string Description { get; set; } 
        public double UsedAmount { get; set; }
        public double UsedPercentage { get; set; }
        public double? MinTargetPercentage { get; set; }
        public double? MaxTargetPercentage { get; set; }
        public double? MinAmount { get; set; }
        public double? MaxAmount { get; set; }
        public TargetMode TargetMode { get; set; }
        public bool IsAchieved { get; set; } 

        /// <summary>
        /// Properties related 
        /// </summary>
        public Guid MonthlyGoalId { get; set; }
        public Guid WalletTypeId { get; set; }
        public virtual MonthlyGoal MonthlyGoal { get; set; }
        public virtual WalletType WalletType { get; set; }
    }
}
