using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_DAL.Entities
{
    public class Jar
    {
        public Jar()
        {
            Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public double TotalSpent { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public Guid CategoryId { get; set; }
        public virtual Category Category { get; set; } = null!;

    }
}
