using MoneyMind_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_BLL.DTOs.GoalItems
{
    public class GoalItemRequest
    {
        public string Description { get; set; }
        public double UsingAmount { get; set; }
        public double UsingPercentage { get; set; }
        public double MinTagetpercentage { get; set; }
        public double MaxTagetpercentage { get; set; }
        public double MinAmount { get; set; }
        public double MaxAmount { get; set; }
        public TargetMode TargetMode { get; set; }
    }
}
