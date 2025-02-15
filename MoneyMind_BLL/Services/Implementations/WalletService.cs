using AutoMapper;
using MoneyMind_BLL.DTOs;
using MoneyMind_BLL.DTOs.MonthlyGoals;
using MoneyMind_BLL.DTOs.WalletCategories;
using MoneyMind_BLL.DTOs.Wallets;
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
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository walletRepository;
        private readonly IMapper mapper;

        public WalletService(IWalletRepository walletRepository, IMapper mapper)
        {
            this.walletRepository = walletRepository;
            this.mapper = mapper;
        }
        public async Task<WalletResponse> AddWalletAsync(Guid userId, WalletRequest walletRequest)
        {
            // Map or Convert DTO to Domain Model
            var walletDomain = mapper.Map<Wallet>(walletRequest);
            walletDomain.UserId = userId;
            // Use Domain Model to create Author
            walletDomain = await walletRepository.InsertAsync(walletDomain);

            return mapper.Map<WalletResponse>(walletDomain);
        }

        public async Task<WalletResponse> DeleteWalletAsync(Guid walletId, Guid userId)
        {
            var existingWallet = await walletRepository.GetByIdAsync(walletId);
            if (existingWallet == null || existingWallet.UserId != userId)
            {
                return null;
            }

            existingWallet.IsActive = false;

            existingWallet = await walletRepository.UpdateAsync(existingWallet);

            return mapper.Map<WalletResponse>(existingWallet);
        }

        public async Task<WalletResponse> GetWalletByIdAsync(Guid walletId)
        {
            var wallet = await walletRepository.GetByIdAsync(walletId);

            if (wallet == null)
            {
                return null;
            }

            return mapper.Map<WalletResponse>(wallet);
        }

        public async Task<ListDataResponse> GetWalletsAsync(Expression<Func<Wallet, bool>>? filter, Func<IQueryable<Wallet>, IOrderedQueryable<Wallet>> orderBy, string includeProperties, int pageIndex, int pageSize)
        {
            var response = await walletRepository.GetAsync(
                        filter: filter,
                        orderBy: orderBy,
                        includeProperties: includeProperties,
                        pageIndex: pageIndex,
                        pageSize: pageSize
                        );
            var wallets = response.Item1;
            var totalPages = response.Item2;
            var totalRecords = response.Item3;

            // Giả sử authorDomains là danh sách các đối tượng AuthorDomain
            var walletTypeResponse = mapper.Map<List<WalletResponse>>(wallets);

            var listResponse = new ListDataResponse
            {
                TotalRecord = totalRecords,
                TotalPage = totalPages,
                PageIndex = pageIndex,
                Data = walletTypeResponse
            };

            return listResponse;
        }

        public async Task<WalletResponse> UpdateWalletAsync(Guid walletId, Guid userId, WalletRequest walletRequest)
        {
            var existingWallet = await walletRepository.GetByIdAsync(walletId);
            if (existingWallet == null || existingWallet.UserId != userId || walletRequest.WalletCategoryId != existingWallet.WalletCategoryId)
            {
                return null;
            }

            existingWallet.Balance = walletRequest.Balance;

            existingWallet = await walletRepository.UpdateAsync(existingWallet);

            return mapper.Map<WalletResponse>(existingWallet);
        }
        public async Task UpdateBalanceAsync(Guid walletId, double amountDifference)
        {
            var wallet = await walletRepository.GetByIdAsync(walletId);
            if (wallet != null)
            {
                wallet.Balance += amountDifference; // Có thể là cộng hoặc trừ
                wallet.LastUpdatedTime = DateTime.Now;
                await walletRepository.UpdateAsync(wallet);
            }
        }
    }
}
