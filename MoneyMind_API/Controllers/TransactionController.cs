using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoneyMind_BLL.DTOs.SubWalletTypes;
using MoneyMind_BLL.DTOs;
using MoneyMind_BLL.Services.Implementations;
using MoneyMind_BLL.Services.Interfaces;
using MoneyMind_BLL.DTOs.Transactions;
using MoneyMind_DAL.Entities;
using System.Linq.Expressions;
using Libook_API.Configure;

namespace MoneyMind_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            this.transactionService = transactionService;
        }

        [HttpGet]
        [Route("{userId:Guid}")]
        [Authorize]
        public async Task<IActionResult> GetAsync(
             [FromRoute] Guid userId,
             [FromQuery] Guid? walletId = null,
             [FromQuery] bool? isCategorized = null, 
             [FromQuery] DateTime? fromDate = null,
             [FromQuery] DateTime? toDate = null,
             [FromQuery] string? orderBy = "CreateAt",
             [FromQuery] bool descending = true,
             [FromQuery] int pageIndex = 1,
             [FromQuery] int pageSize = 12)
        {
            // Đảm bảo pageIndex và pageSize hợp lệ
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize < 1) pageSize = 12;

            // Tạo bộ lọc
            Expression<Func<Transaction, bool>> filterExpression = s => s.IsActive && s.UserId == userId;

            if (walletId.HasValue)
            {
                filterExpression = filterExpression.AndAlso(s => s.WalletId == walletId.Value);
            }
            if (fromDate.HasValue)
            {
                filterExpression = filterExpression.AndAlso(s => s.TransactionDate >= fromDate.Value);
            }
            if (toDate.HasValue)
            {
                filterExpression = filterExpression.AndAlso(s => s.TransactionDate <= toDate.Value);
            }
            if (isCategorized.HasValue)
            {
                if (isCategorized.Value)
                {
                    filterExpression = filterExpression.AndAlso(s => s.WalletId != null); 
                }
                else
                {
                    filterExpression = filterExpression.AndAlso(s => s.WalletId == null);
                }
            }

            // Tạo sắp xếp linh hoạt
            Func<IQueryable<Transaction>, IOrderedQueryable<Transaction>> orderByFunc = descending
                ? q => q.OrderByDescending(GetOrderByProperty(orderBy))
                : q => q.OrderBy(GetOrderByProperty(orderBy));

            // Lấy dữ liệu
            var listDataResponse = await transactionService.GetTransactionAsync(
                filter: filterExpression,
                orderBy: orderByFunc,
                includeProperties: "TransactionTags.Tag,TransactionActivities.Activity",
                pageIndex: pageIndex,
                pageSize: pageSize
            );

            var response = new ResponseObject
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = "Get transaction of user successfully!",
                Data = listDataResponse
            };

            return Ok(response);
        }

        private static Expression<Func<Transaction, object>> GetOrderByProperty(string? orderBy)
        {
            return orderBy?.ToLower() switch
            {
                "amount" => t => t.Amount,
                "recipientname" => t => t.RecipientName,
                "description" => t => t.Description,
                "transactiondate" => t => t.TransactionDate,
                "createdat" => t => t.CreateAt,
                _ => t => t.CreateAt 
            };
        }

        [HttpGet]
        [Route("detail/{trasactionId:Guid}")]
        public async Task<IActionResult> GetMonthlyGoalByIdAsync([FromRoute] Guid trasactionId)
        {
            var transaction = await transactionService.GetTransactionByIdAsync(trasactionId);
            if (transaction == null)
            {
                return NotFound(new ResponseObject
                {
                    Status = System.Net.HttpStatusCode.NotFound,
                    Message = "Transaction not found!",
                    Data = null
                });
            }

            return Ok(new ResponseObject
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = "Get monthly goal successfully!",
                Data = transaction
            });
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CreateAsync(TransactionRequest transactionRequest)
        {
            // Sử dụng helper để lấy UserId từ token
            var userId = JwtHelper.GetUserIdFromToken(HttpContext.Request, out var errorMessage);

            if (userId == null)
            {
                // Trả về lỗi nếu token không hợp lệ
                return Unauthorized(errorMessage);
            }

            var transactionResponse = await transactionService.AddTransactionAsync(userId.Value, transactionRequest);

            var response = new ResponseObject
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = "Create transaction successfully !",
                Data = transactionResponse
            };
            return Ok(response);
        }

        [HttpPut]
        [Route("{id:Guid}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] TransactionRequest transactionRequest)
        {
            // Sử dụng helper để lấy UserId từ token
            var userId = JwtHelper.GetUserIdFromToken(HttpContext.Request, out var errorMessage);

            if (userId == null)
            {
                // Trả về lỗi nếu token không hợp lệ
                return Unauthorized(errorMessage);
            }

            var transactionResponse = await transactionService.UpdateTransactionAsync(id, userId.Value, transactionRequest);
            if (transactionResponse == null)
            {
                return NotFound();
            }
            var response = new ResponseObject
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = "Update sub wallet types successfully !",
                Data = transactionResponse
            };
            return Ok(response);
        }

        [HttpPut]
        [Route("{id:Guid}/delete")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
        {
            // Sử dụng helper để lấy UserId từ token
            var userId = JwtHelper.GetUserIdFromToken(HttpContext.Request, out var errorMessage);

            if (userId == null)
            {
                // Trả về lỗi nếu token không hợp lệ
                return Unauthorized(errorMessage);
            }

            var transactionResponse = await transactionService.DeleteTransactionAsync(id, userId.Value);
            if (transactionResponse == null)
            {
                return NotFound();
            }
            var response = new ResponseObject
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = "Delete transaction successfully !",
                Data = transactionResponse
            };
            return Ok(response);
        }
    }
}
