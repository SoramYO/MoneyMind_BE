using MoneyMind_BLL.DTOs.Wallets;
using MoneyMind_BLL.DTOs;
using MoneyMind_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MoneyMind_BLL.DTOs.Transactions;
using MoneyMind_BLL.DTOs.WalletCategories;
using MoneyMind_BLL.DTOs.MonthlyGoals;

namespace MoneyMind_BLL.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<ListDataResponse> GetTransactionAsync(
            Expression<Func<Transaction, bool>>? filter,
            Func<IQueryable<Transaction>, IOrderedQueryable<Transaction>> orderBy,
            string includeProperties,
            int pageIndex,
            int pageSize);
        Task<TransactionResponse> AddTransactionAsync(Guid userId, TransactionRequest transactionRequest);
        Task<TransactionResponse> UpdateTransactionAsync(Guid transactionId, Guid userId, TransactionRequest transactionRequest);
        Task<TransactionResponse> DeleteTransactionAsync(Guid transactionId, Guid userId);
        Task<TransactionResponse> GetTransactionByIdAsync(Guid transactionId);
    }
}
