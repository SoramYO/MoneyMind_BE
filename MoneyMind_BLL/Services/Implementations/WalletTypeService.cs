using AutoMapper;
using MoneyMind_BLL.DTOs;
using MoneyMind_BLL.DTOs.WalletTypes;
using MoneyMind_BLL.Services.Interfaces;
using MoneyMind_DAL.Entities;
using MoneyMind_DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_BLL.Services.Implementations
{
    public class WalletTypeService : IWalletTypeService
    {
        private readonly IWalletTypeRepository walletTypeRepository;
        private readonly IMapper mapper;

        public WalletTypeService(IWalletTypeRepository walletTypeRepository, IMapper mapper)
        {
            this.walletTypeRepository = walletTypeRepository;
            this.mapper = mapper;
        }
        public async Task<ListDataResponse> GetWalletTypesAsync(
            Expression<Func<WalletType, bool>>? filter,
            Func<IQueryable<WalletType>, IOrderedQueryable<WalletType>> orderBy,
            string includeProperties,
            int pageIndex,
            int pageSize)
        {
            var response = await walletTypeRepository.GetAsync(
                        filter: filter,
                        orderBy: orderBy,
                        includeProperties: includeProperties,
                        pageIndex: pageIndex,
                        pageSize: pageSize
                        );
            var walletTypes = response.Item1;
            var totalPages = response.Item2;
            var totalRecords = response.Item3;

            // Giả sử authorDomains là danh sách các đối tượng AuthorDomain
            var walletTypeResponse = mapper.Map<List<WalletTypeResponse>>(walletTypes);

            var listResponse = new ListDataResponse
            {
                TotalRecord = totalRecords,
                TotalPage = totalPages,
                PageIndex = pageIndex,
                Data = walletTypeResponse
            };

            return listResponse;
        }
    }
}
