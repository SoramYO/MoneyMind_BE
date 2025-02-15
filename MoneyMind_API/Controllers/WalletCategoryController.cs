using Libook_API.Configure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MoneyMind_BLL.DTOs;
using MoneyMind_BLL.DTOs.WalletCategories;
using MoneyMind_BLL.Services.Implementations;
using MoneyMind_BLL.Services.Interfaces;
using MoneyMind_DAL.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;

namespace MoneyMind_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletCategoryController : ControllerBase
    {
        private readonly IWalletCategoryService walletCategoryService;

        public WalletCategoryController(IWalletCategoryService walletCategoryService)
        {
            this.walletCategoryService = walletCategoryService;
        }

        [HttpGet]
        [Route("{userId:Guid}")]
        [Authorize]
        public async Task<IActionResult> GetAsync(
            [FromRoute] Guid userId,
            [FromQuery] Guid? walletTypeId = null,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 12)
        {
            // Đảm bảo pageIndex và pageSize hợp lệ
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize < 1) pageSize = 12;

            // Start with a base filter
            Expression<Func<WalletCategory, bool>> filterExpression = s => s.IsActive;
            filterExpression = filterExpression.AndAlso(s => s.UserId == userId);

            //Combine filters using AndAlso method
            if (walletTypeId.HasValue)
            {
                filterExpression = filterExpression.AndAlso(s => s.WalletTypeId == walletTypeId);
            }

            // Map string orderBy to the appropriate property (use switch or reflection if necessary)
            Func<IQueryable<WalletCategory>, IOrderedQueryable<WalletCategory>> orderByFunc = q => q.OrderBy(s => s.CreateAt);

            var listDataResponse = await walletCategoryService.GetWalletCategoriesAsync(
                filter: filterExpression,
                orderBy: orderByFunc,
                includeProperties: "WalletType,Activities",
                pageIndex: pageIndex,
                pageSize: pageSize
            );

            var response = new ResponseObject
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = "Get wallet category of user successfully!",
                Data = listDataResponse
            };

            return Ok(response);
        }

        [HttpGet]
        [Route("detail/{walletCategoryId:Guid}")]
        public async Task<IActionResult> GetMonthlyGoalByIdAsync([FromRoute] Guid walletCategoryId)
        {
            var walletCategory = await walletCategoryService.GetWalletCategoryByIdAsync(walletCategoryId);
            if (walletCategory == null)
            {
                return NotFound(new ResponseObject
                {
                    Status = System.Net.HttpStatusCode.NotFound,
                    Message = "Wallet category type not found!",
                    Data = null
                });
            }

            return Ok(new ResponseObject
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = "Get wallet category successfully!",
                Data = walletCategory
            });
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CreateAsync(WalletCategoryRequest walletCategoryRequest)
        {
            // Sử dụng helper để lấy UserId từ token
            var userId = JwtHelper.GetUserIdFromToken(HttpContext.Request, out var errorMessage);

            if (userId == null)
            {
                // Trả về lỗi nếu token không hợp lệ
                return Unauthorized(errorMessage);
            }

            var walletCategoryResponse = await walletCategoryService.AddWalletCategoryAsync(userId.Value, walletCategoryRequest);

            var response = new ResponseObject
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = "Create wallet category successfully !",
                Data = walletCategoryResponse
            };
            return Ok(response);
        }

        [HttpPost]
        [Route("create-default")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CreateDefaultWalletCategoriesAsync()
        {

            // Sử dụng helper để lấy UserId từ token
            var userId = JwtHelper.GetUserIdFromToken(HttpContext.Request, out var errorMessage);

            if (userId == null)
            {
                // Trả về lỗi nếu token không hợp lệ
                return Unauthorized(errorMessage);
            }
            var walletCategoryResponse = await walletCategoryService.CreateDefaultWalletCategoriesAndActivitiesAsync(
                userId.Value
            );
            var response = new ResponseObject
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = "Create wallet categories default successfully !",
                Data = walletCategoryResponse
            };
            return Ok(response);
        }

        [HttpPut]
        [Route("{id:Guid}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] WalletCategoryRequest walletCategoryRequest)
        {
            // Sử dụng helper để lấy UserId từ token
            var userId = JwtHelper.GetUserIdFromToken(HttpContext.Request, out var errorMessage);

            if (userId == null)
            {
                // Trả về lỗi nếu token không hợp lệ
                return Unauthorized(errorMessage);
            }

            var walletCategoryResponse = await walletCategoryService.UpdateWalletCategoryAsync(id, userId.Value, walletCategoryRequest);
            if (walletCategoryResponse == null)
            {
                return NotFound();
            }
            var response = new ResponseObject
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = "Update wallet category successfully!",
                Data = walletCategoryResponse
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

            var walletCategoryResponse = await walletCategoryService.DeleteWalletCategoryAsync(id, userId.Value);
            if (walletCategoryResponse == null)
            {
                return NotFound();
            }
            var response = new ResponseObject
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = "Delete wallet category successfully !",
                Data = walletCategoryResponse
            };
            return Ok(response);
        }
    }
}
