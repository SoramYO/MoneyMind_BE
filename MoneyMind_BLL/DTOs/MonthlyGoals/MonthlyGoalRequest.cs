using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_BLL.DTOs.MonthlyGoals
{
    public class MonthlyGoalRequest
    {
        public double TotalAmount { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public DateTime CreateAt { get; set; }
        public bool IsCompleted { get; set; }
    }
}
