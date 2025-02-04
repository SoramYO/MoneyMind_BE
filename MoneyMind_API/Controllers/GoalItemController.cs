using Libook_API.Configure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoneyMind_BLL.DTOs;
using MoneyMind_BLL.DTOs.GoalItems;
using MoneyMind_BLL.DTOs.Transactions;
using MoneyMind_BLL.Services.Implementations;
using MoneyMind_BLL.Services.Interfaces;
using MoneyMind_DAL.Entities;
using System.Linq.Expressions;

namespace MoneyMind_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoalItemController : ControllerBase
    {
        private readonly IGoalItemService goalItemService;

        public GoalItemController(IGoalItemService goalItemService)
        {
            this.goalItemService = goalItemService;
        }

        [HttpGet]
        [Route("{userId:Guid}")]
        [Authorize]
        public async Task<IActionResult> GetAsync(
             [FromRoute] Guid userId,
             [FromQuery] Guid? monthlyGoalId = null,
             [FromQuery] Guid? walletTypeId = null,
             [FromQuery] TargetMode? targetMode = null,
             [FromQuery] bool? isAchieved = null,
             [FromQuery] int? month = null,
             [FromQuery] int? year = null,
             [FromQuery] int pageIndex = 1,
             [FromQuery] int pageSize = 12)
        {
            // Đảm bảo pageIndex và pageSize hợp lệ
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize < 1) pageSize = 12;

            // Tạo bộ lọc
            Expression<Func<GoalItem, bool>> filterExpression = g => g.MonthlyGoal.UserId == userId;

            if (monthlyGoalId.HasValue)
            {
                filterExpression = filterExpression.AndAlso(g => g.MonthlyGoalId == monthlyGoalId.Value);
            }
            if (walletTypeId.HasValue)
            {
                filterExpression = filterExpression.AndAlso(g => g.WalletTypeId == walletTypeId.Value);
            }
            if (targetMode.HasValue)
            {
                filterExpression = filterExpression.AndAlso(g => g.TargetMode == targetMode.Value);
            }
            if (isAchieved.HasValue)
            {
                filterExpression = filterExpression.AndAlso(g => g.IsAchieved == isAchieved.Value);
            }
            if (month.HasValue)
            {
                filterExpression = filterExpression.AndAlso(g => g.MonthlyGoal.Month == month);
            }
            if (year.HasValue)
            {
                filterExpression = filterExpression.AndAlso(g => g.MonthlyGoal.Year == year);
            }


            // Lấy dữ liệu
            var listDataResponse = await goalItemService.GetGoalItemAsync(
                filter: filterExpression,
                orderBy: null,
                includeProperties: "MonthlyGoal,WalletType",
                pageIndex: pageIndex,
                pageSize: pageSize
            );

            var response = new ResponseObject
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = "Get goal items successfully!",
                Data = listDataResponse
            };

            return Ok(response);
        }


        [HttpGet]
        [Route("detail/{goalItemId:Guid}")]
        public async Task<IActionResult> GetMonthlyGoalByIdAsync([FromRoute] Guid goalItemId)
        {
            var goalItem = await goalItemService.GetGoalItemByIdAsync(goalItemId);
            if (goalItem == null)
            {
                return NotFound(new ResponseObject
                {
                    Status = System.Net.HttpStatusCode.NotFound,
                    Message = "Goal not found!",
                    Data = null
                });
            }

            return Ok(new ResponseObject
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = "Get goal successfully!",
                Data = goalItem
            });
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CreateAsync(GoalItemRequest goalItemRequest)
        {
            // Sử dụng helper để lấy UserId từ token
            var userId = JwtHelper.GetUserIdFromToken(HttpContext.Request, out var errorMessage);

            if (userId == null)
            {
                // Trả về lỗi nếu token không hợp lệ
                return Unauthorized(errorMessage);
            }

            var goalItemResponse = await goalItemService.AddGoalItemAsync(userId.Value ,goalItemRequest);

            if(goalItemResponse == null)
            {
                return BadRequest(new { Message = "Goal item with the same wallet type already exists or invalid monthly goal." });
            }

            var response = new ResponseObject
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = "Create goal item successfully !",
                Data = goalItemResponse
            };
            return Ok(response);
        }

        [HttpPut]
        [Route("{id:Guid}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] GoalItemRequest goalItemRequest)
        {
            // Sử dụng helper để lấy UserId từ token
            var userId = JwtHelper.GetUserIdFromToken(HttpContext.Request, out var errorMessage);

            if (userId == null)
            {
                // Trả về lỗi nếu token không hợp lệ
                return Unauthorized(errorMessage);
            }

            var transactionResponse = await goalItemService.UpdateGoalItemAsync(id, userId.Value, goalItemRequest);
            if (transactionResponse == null)
            {
                return BadRequest(new { Message = "Goal item with the same wallet type already exists or invalid monthly goal." });
            }
            var response = new ResponseObject
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = "Update goal item successfully !",
                Data = transactionResponse
            };
            return Ok(response);
        }
    }
}
