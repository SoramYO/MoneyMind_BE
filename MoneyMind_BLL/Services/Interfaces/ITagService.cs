using MoneyMind_BLL.DTOs;
using MoneyMind_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_BLL.Services.Interfaces
{
    public interface ITagService
    {
        Task<ListDataResponse> GetTagsAsync(
            Expression<Func<Tag, bool>>? filter,
            Func<IQueryable<Tag>, IOrderedQueryable<Tag>> orderBy,
            string includeProperties,
            int pageIndex,
            int pageSize);
    }
}
