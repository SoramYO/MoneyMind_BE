using AutoMapper;
using MoneyMind_BLL.DTOs;
using MoneyMind_BLL.DTOs.MonthlyGoals;
using MoneyMind_BLL.DTOs.SubWalletTypes;
using MoneyMind_BLL.DTOs.Transactions;
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
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository transactionRepository;
        private readonly IMapper mapper;
        private readonly IMLService mlService;
        private readonly ITransactionTagRepository transactionTagRepository;

        public TransactionService(ITransactionRepository transactionRepository, IMapper mapper, IMLService mLService, ITransactionTagRepository transactionTagRepository)
        {
            this.transactionRepository = transactionRepository;
            this.mapper = mapper;
            this.mlService = mLService;
            this.transactionTagRepository = transactionTagRepository;
        }
        public async Task<TransactionResponse> AddTransactionAsync(Guid userId, TransactionRequest transactionRequest)
        {
            // Map or Convert DTO to Domain Model
            var transactionDomain = mapper.Map<Transaction>(transactionRequest);
            transactionDomain.UserId = userId;
            var tag = await mlService.ClassificationTag(
                transactionDomain.Description
            );
            transactionDomain.TransactionTags = new List<TransactionTag>
            {
                new TransactionTag
                {
                    TagId = tag.Id
                }
            };
            
            // Use Domain Model to create
            transactionDomain = await transactionRepository.InsertAsync(transactionDomain);

            return mapper.Map<TransactionResponse>(transactionDomain);
        }

        public async Task<TransactionResponse> DeleteTransactionAsync(Guid transactionId, Guid userId)
        {
            var existingTransaction = await transactionRepository.GetByIdAsync(transactionId);
            if (existingTransaction == null || existingTransaction.UserId != userId)
            {
                return null;
            }

            existingTransaction.IsActive = false;

            existingTransaction = await transactionRepository.UpdateAsync(existingTransaction);

            return mapper.Map<TransactionResponse>(existingTransaction);
        }

        public async Task<ListDataResponse> GetTransactionAsync(Expression<Func<Transaction, bool>>? filter, Func<IQueryable<Transaction>, IOrderedQueryable<Transaction>> orderBy, string includeProperties, int pageIndex, int pageSize)
        {
            var response = await transactionRepository.GetAsync(
                        filter: filter,
                        orderBy: orderBy,
                        includeProperties: includeProperties,
                        pageIndex: pageIndex,
                        pageSize: pageSize
                        );
            var transactions = response.Item1;
            var totalPages = response.Item2;
            var totalRecords = response.Item3;

            // Giả sử authorDomains là danh sách các đối tượng AuthorDomain
            var transactionResponse = mapper.Map<List<TransactionResponse>>(transactions);

            var listResponse = new ListDataResponse
            {
                TotalRecord = totalRecords,
                TotalPage = totalPages,
                PageIndex = pageIndex,
                Data = transactionResponse
            };

            return listResponse;
        }

        public async Task<TransactionResponse> GetTransactionByIdAsync(Guid transactionId)
        {
            var transaction = await transactionRepository.GetByIdAsync(transactionId, t => t.TransactionTags);

            if (transaction == null)
            {
                return null;
            }

            return mapper.Map<TransactionResponse>(transaction);
        }

        public async Task<TransactionResponse> UpdateTransactionAsync(Guid transactionId, Guid userId, TransactionRequest transactionRequest)
        {
            var existingTransaction = await transactionRepository.GetByIdAsync(transactionId);
            if (existingTransaction == null || existingTransaction.UserId != userId)
            {
                return null;
            }

            existingTransaction.RecipientName = transactionRequest.RecipientName;
            existingTransaction.Amount = transactionRequest.Amount;
            existingTransaction.Description = transactionRequest.Description;
            existingTransaction.TransactionDate = transactionRequest.TransactionDate;
            existingTransaction.WalletId = transactionRequest.WalletId;
            existingTransaction.UpdatedAt = DateTime.Now;

            await transactionTagRepository.DeleteAllByTransactionId(transactionId);

            var tag = await mlService.ClassificationTag(existingTransaction.Description);

            existingTransaction.TransactionTags = new List<TransactionTag>
            {
                new TransactionTag { TagId = tag.Id }
            };

            existingTransaction = await transactionRepository.UpdateAsync(existingTransaction);

            return mapper.Map<TransactionResponse>(existingTransaction);
        }
    }
}
