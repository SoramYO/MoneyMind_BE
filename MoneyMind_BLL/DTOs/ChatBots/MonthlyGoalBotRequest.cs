using MoneyMind_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_BLL.DTOs.ChatBots
{
    public class MonthlyGoalBotRequest
    {
        public GoalStatus Status { get; set; }
        public double TotalAmount { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public bool IsCompleted { get; set; }
        public  List<GoalItemBotRequest> GoalItems { get; set; } = new List<GoalItemBotRequest>();
    }
}
