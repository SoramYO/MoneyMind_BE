using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_BLL.DTOs.GoalItems
{
    public class GoalItemDefault
    {
        public string Description { get; set; } = string.Empty;
        public double? MinTargetPercentage { get; set; }
        public double? MaxTargetPercentage { get; set; }
        public double? MinAmount { get; set; }
        public double? MaxAmount { get; set; }
    }
}
