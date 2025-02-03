using Libook_API.Configure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoneyMind_BLL.DTOs;
using MoneyMind_BLL.DTOs.SubWalletTypes;
using MoneyMind_BLL.DTOs.Wallets;
using MoneyMind_BLL.Services.Implementations;
using MoneyMind_BLL.Services.Interfaces;
using MoneyMind_DAL.Entities;
using System.Linq.Expressions;

namespace MoneyMind_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService walletService;

        public WalletController(IWalletService walletService)
        {
            this.walletService = walletService;
        }

        [HttpGet]
        [Route("{userId:Guid}")]
        [Authorize]
        public async Task<IActionResult> GetAsync(
            [FromRoute] Guid userId,
            [FromQuery] Guid? subWalletTypeId = null,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 12)
        {
            // Đảm bảo pageIndex và pageSize hợp lệ
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize < 1) pageSize = 12;

            // Start with a base filter
            Expression<Func<Wallet, bool>> filterExpression = s => s.IsActive;
            filterExpression = filterExpression.AndAlso(s => s.UserId == userId);

            // Combine filters using AndAlso method
            if (subWalletTypeId.HasValue)
            {
                filterExpression = filterExpression.AndAlso(s => s.SubWalletTypeId == subWalletTypeId);
            }

            // Map string orderBy to the appropriate property (use switch or reflection if necessary)
            Func<IQueryable<Wallet>, IOrderedQueryable<Wallet>> orderByFunc = q => q.OrderBy(s => s.CreatedTime);

            var listDataResponse = await walletService.GetWalletsAsync(
                filter: filterExpression,
                orderBy: orderByFunc,
                includeProperties: "",
                pageIndex: pageIndex,
                pageSize: pageSize
            );

            var response = new ResponseObject
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = "Get wallet of user successfully!",
                Data = listDataResponse
            };

            return Ok(response);
        }

        [HttpGet]
        [Route("detail/{walletId:Guid}")]
        public async Task<IActionResult> GetMonthlyGoalByIdAsync([FromRoute] Guid walletId)
        {
            var wallet = await walletService.GetWalletByIdAsync(walletId);
            if (wallet == null)
            {
                return NotFound(new ResponseObject
                {
                    Status = System.Net.HttpStatusCode.NotFound,
                    Message = "Wallet not found!",
                    Data = null
                });
            }

            return Ok(new ResponseObject
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = "Get wallet successfully!",
                Data = wallet
            });
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CreateAsync(WalletRequest walletRequest)
        {
            // Sử dụng helper để lấy UserId từ token
            var userId = JwtHelper.GetUserIdFromToken(HttpContext.Request, out var errorMessage);

            if (userId == null)
            {
                // Trả về lỗi nếu token không hợp lệ
                return Unauthorized(errorMessage);
            }

            var subWalletTypeResponse = await walletService.AddWalletAsync(userId.Value, walletRequest);

            var response = new ResponseObject
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = "Create wallet successfully !",
                Data = subWalletTypeResponse
            };
            return Ok(response);
        }

        [HttpPut]
        [Route("{id:Guid}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] WalletRequest walletRequest)
        {
            // Sử dụng helper để lấy UserId từ token
            var userId = JwtHelper.GetUserIdFromToken(HttpContext.Request, out var errorMessage);

            if (userId == null)
            {
                // Trả về lỗi nếu token không hợp lệ
                return Unauthorized(errorMessage);
            }

            var walletResponse = await walletService.UpdateWalletAsync(id, userId.Value, walletRequest);
            if (walletResponse == null)
            {
                return NotFound();
            }
            var response = new ResponseObject
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = "Update wallet successfully !",
                Data = walletResponse
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

            var walletResponse = await walletService.DeleteWalletAsync(id, userId.Value);
            if (walletResponse == null)
            {
                return NotFound();
            }
            var response = new ResponseObject
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = "Delete wallet successfully !",
                Data = walletResponse
            };
            return Ok(response);
        }
    }
}
