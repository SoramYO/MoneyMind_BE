using Microsoft.EntityFrameworkCore.Storage.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_DAL.Entities
{
    public enum GoalStatus
    {
        InProgress = 1,     // Đang thực hiện
        Completed = 2,      // Hoàn thành
    }
    public class MonthlyGoal
    {
        public MonthlyGoal()
        {
            Id = Guid.NewGuid();
            Status = GoalStatus.InProgress;
            CreateAt = DateTime.UtcNow;
            IsCompleted = false;
        }
        public Guid Id { get; set; }
        public GoalStatus Status { get; set; }
        public double TotalAmount { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public DateTime CreateAt { get; set; }
        public bool IsCompleted { get; set; } 

        /// <summary>
        /// Properties related 
        /// </summary>
        public Guid UserId { get; set; }
        public virtual ICollection<GoalItem> GoalItems { get; set; } = new List<GoalItem>();
    }
}
