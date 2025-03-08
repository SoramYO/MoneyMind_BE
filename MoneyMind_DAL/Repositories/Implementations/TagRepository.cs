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
    public class TagRepository : GenericRepository<Tag>, ITagRepository
    {
        public TagRepository(MoneyMindDbContext context) : base(context)
        {
        }

        public async Task<Tag> GetByName(string name)
        {
            return await _context.Tag.FirstOrDefaultAsync(x => x.Name == name);
        }
    }
}
