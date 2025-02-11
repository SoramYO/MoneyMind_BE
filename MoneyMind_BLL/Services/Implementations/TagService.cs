using AutoMapper;
using MoneyMind_BLL.DTOs;
using MoneyMind_BLL.DTOs.Tags;
using MoneyMind_BLL.DTOs.WalletTypes;
using MoneyMind_BLL.Services.Interfaces;
using MoneyMind_DAL.Entities;
using MoneyMind_DAL.Repositories.Implementations;
using MoneyMind_DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_BLL.Services.Implementations
{
    public class TagService : ITagService
    {
        private readonly ITagRepository tagRepository;
        private readonly IMapper mapper;

        public TagService(ITagRepository tagRepository, IMapper mapper)
        {
            this.tagRepository = tagRepository;
            this.mapper = mapper;
        }
        public async Task<ListDataResponse> GetTagsAsync(Expression<Func<Tag, bool>>? filter, Func<IQueryable<Tag>, IOrderedQueryable<Tag>> orderBy, string includeProperties, int pageIndex, int pageSize)
        {
            var response = await tagRepository.GetAsync(
                        filter: filter,
                        orderBy: orderBy,
                        includeProperties: includeProperties,
                        pageIndex: pageIndex,
                        pageSize: pageSize
                        );
            var tags = response.Item1;
            var totalPages = response.Item2;
            var totalRecords = response.Item3;

            // Giả sử authorDomains là danh sách các đối tượng AuthorDomain
            var tagsResponse = mapper.Map<List<TagResponse>>(tags);

            var listResponse = new ListDataResponse
            {
                TotalRecord = totalRecords,
                TotalPage = totalPages,
                PageIndex = pageIndex,
                Data = tagsResponse
            };

            return listResponse;
        }
    }
}
