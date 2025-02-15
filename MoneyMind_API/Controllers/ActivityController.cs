using Libook_API.Configure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyMind_BLL.DTOs;
using MoneyMind_BLL.DTOs.Activities;
using MoneyMind_BLL.Services.Interfaces;
using MoneyMind_DAL.Entities;
using System.Linq.Expressions;

namespace MoneyMind_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityController : ControllerBase
    {
        private readonly IActivityService activityService;

        public ActivityController(IActivityService activityService)
        {
            this.activityService = activityService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] Guid? WalletCategoryId = null,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 50)
        {
            // Đảm bảo pageIndex và pageSize hợp lệ
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize < 1) pageSize = 50;

            Expression<Func<Activity, bool>> filterExpression = s => s.IsDeleted == false;
            if (WalletCategoryId.HasValue)
            {
                filterExpression = filterExpression.AndAlso(s => s.WalletCategoryId == WalletCategoryId);
            }

            var listDataResponse = await activityService.GetActivityAsync(
                filter: filterExpression,
                orderBy: null,
                includeProperties: "WalletCategory.WalletType",
                pageIndex: pageIndex,
                pageSize: pageSize
            );

            var response = new ResponseObject
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = "Get all activity successfully!",
                Data = listDataResponse
            };

            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CreateAsync(ActivityRequest activityRequest)
        {
            // Sử dụng helper để lấy UserId từ token
            var userId = JwtHelper.GetUserIdFromToken(HttpContext.Request, out var errorMessage);

            if (userId == null)
            {
                // Trả về lỗi nếu token không hợp lệ
                return Unauthorized(errorMessage);
            }

            var activityResponse = await activityService.AddActivityAsync(userId.Value, activityRequest);

            var response = new ResponseObject
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = "Create activity successfully !",
                Data = activityResponse
            };
            return Ok(response);
        }

        [HttpPut]
        [Route("{id:Guid}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] ActivityRequest activityRequest)
        {
            // Sử dụng helper để lấy UserId từ token
            var userId = JwtHelper.GetUserIdFromToken(HttpContext.Request, out var errorMessage);

            if (userId == null)
            {
                // Trả về lỗi nếu token không hợp lệ
                return Unauthorized(errorMessage);
            }

            var activityResponse = await activityService.UpdateActivityAsync(userId.Value, id, activityRequest);
            if (activityResponse == null)
            {
                return NotFound();
            }
            var response = new ResponseObject
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = "Update activity successfully !",
                Data = activityResponse
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

            var activityResponse = await activityService.DeleteActivityAsync(userId.Value, id);
            if (activityResponse == null)
            {
                return NotFound();
            }
            var response = new ResponseObject
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = "Delete activity successfully !",
                Data = activityResponse
            };
            return Ok(response);
        }
    }
}
