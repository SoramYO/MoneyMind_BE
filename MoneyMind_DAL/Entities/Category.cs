using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_DAL.Entities
{
    public class Category
    {
        public Category()
        {
            Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;

        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
