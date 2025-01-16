using Microsoft.EntityFrameworkCore.Storage.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_DAL.Entities
{
    public class MonthlyGoal
    {
        public MonthlyGoal()
        {
            Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }
        public double TargetAmount { get; set; }
        public double SpentAmount { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public Guid UserId { get; set; }
    }
}
