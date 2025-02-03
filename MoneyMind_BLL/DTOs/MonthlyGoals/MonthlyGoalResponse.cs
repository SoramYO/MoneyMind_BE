using MoneyMind_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_BLL.DTOs.MonthlyGoals
{
    public class MonthlyGoalResponse
    {
        public Guid Id { get; set; }
        public GoalStatus Status { get; set; }
        public double TotalAmount { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public DateTime CreateAt { get; set; }
        public bool IsCompleted { get; set; }
    }
}
