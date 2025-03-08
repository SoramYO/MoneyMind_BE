using AutoMapper;
using MoneyMind_BLL.DTOs;
using MoneyMind_BLL.DTOs.Wallets;
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
    public class WalletTypeService : IWalletTypeService
    {
        private readonly IWalletTypeRepository walletTypeRepository;
        private readonly IMapper mapper;

        public WalletTypeService(IWalletTypeRepository walletTypeRepository, IMapper mapper)
        {
            this.walletTypeRepository = walletTypeRepository;
            this.mapper = mapper;
        }

        public async Task<WalletTypeResponse> AddWalletTypeAsync(WalletTypeRequest walletTypeRequest)
        {
            // Map or Convert DTO to Domain Model
            var walletTypeDomain = mapper.Map<WalletType>(walletTypeRequest);
            // Use Domain Model to create Author
            walletTypeDomain = await walletTypeRepository.InsertAsync(walletTypeDomain);

            return mapper.Map<WalletTypeResponse>(walletTypeDomain);
        }

        public async Task<WalletTypeResponse> DeleteWalletTypeAsync(Guid walletTypeId)
        {
            var existingWalletType = await walletTypeRepository.GetByIdAsync(walletTypeId);
            if (existingWalletType == null)
            {
                return null;
            }

            existingWalletType.IsDisabled = true;

            existingWalletType = await walletTypeRepository.UpdateAsync(existingWalletType);

            return mapper.Map<WalletTypeResponse>(existingWalletType);
        }

        public async Task<WalletTypeResponse> GetWalletTypeByIdAsync(Guid walletTypeId)
        {
            var existingWalletType = await walletTypeRepository.GetByIdAsync(walletTypeId);

            if (existingWalletType == null)
            {
                return null;
            }

            return mapper.Map<WalletTypeResponse>(existingWalletType);
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

        public async Task<WalletTypeResponse> UpdateWalletTypeAsync(Guid walletTypeId, WalletTypeRequest walletTypeRequest)
        {
            var existingWalletType = await walletTypeRepository.GetByIdAsync(walletTypeId);
            if (existingWalletType == null)
            {
                return null;
            }

            existingWalletType.Name = walletTypeRequest.Name;
            existingWalletType.Description = walletTypeRequest.Description;

            existingWalletType = await walletTypeRepository.UpdateAsync(existingWalletType);

            return mapper.Map<WalletTypeResponse>(existingWalletType);
        }
    }
}
