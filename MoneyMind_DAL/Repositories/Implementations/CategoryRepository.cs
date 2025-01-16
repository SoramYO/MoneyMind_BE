using Microsoft.EntityFrameworkCore;
using MoneyMind_DAL.DBContexts;
using MoneyMind_DAL.Entities;
using MoneyMind_DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_DAL.Repositories.Implementations
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(MoneyMindDbContext context) : base(context)
        {
        }

        public async Task<Category> GetByName(string name)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.Name.Equals(name));
        }
    }
}
