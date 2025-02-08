using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoneyMind_BLL.DTOs.SubWalletTypes;
using MoneyMind_BLL.DTOs;
using MoneyMind_BLL.Services.Implementations;
using MoneyMind_BLL.Services.Interfaces;
using MoneyMind_BLL.DTOs.MonthlyGoals;
using MoneyMind_DAL.Entities;
using System.Linq.Expressions;
using Libook_API.Configure;

namespace MoneyMind_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MonthlyGoalController : ControllerBase
    {
        private readonly IMonthlyGoalService monthlyGoalService;

        public MonthlyGoalController(IMonthlyGoalService monthlyGoalService)
        {
            this.monthlyGoalService = monthlyGoalService;
        }
        [HttpGet]
        [Route("{userId:Guid}")]
        public async Task<IActionResult> GetUserMonthlyGoalsAsync(
            [FromRoute] Guid userId,
            [FromQuery] int? year = null,
            [FromQuery] int? month = null,
            [FromQuery] GoalStatus? status = null,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 12)
        {
            // Đảm bảo pageIndex và pageSize hợp lệ
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize < 1) pageSize = 12;

            // Tạo bộ lọc động
            Expression<Func<MonthlyGoal, bool>> filterExpression = g => g.UserId == userId;
            if (year.HasValue)
            {
                filterExpression = filterExpression.AndAlso(g => g.Year == year.Value);
            }
            if (month.HasValue)
            {
                filterExpression = filterExpression.AndAlso(g => g.Month == month.Value);
            }
            if (status.HasValue)
            {
                filterExpression = filterExpression.AndAlso(g => g.Status == status.Value);
            }

            // Sắp xếp theo thời gian tạo
            Func<IQueryable<MonthlyGoal>, IOrderedQueryable<MonthlyGoal>> orderByFunc = q => q.OrderByDescending(g => g.CreateAt);

            // Gọi service lấy dữ liệu
            var listDataResponse = await monthlyGoalService.GetMonthlyGoalAsync(
                filter: filterExpression,
                orderBy: orderByFunc,
                includeProperties: "GoalItems.WalletType",
                pageIndex: pageIndex,
                pageSize: pageSize
            );

            var response = new ResponseObject
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = "Get monthly goals successfully!",
                Data = listDataResponse
            };

            return Ok(response);
        }
        [HttpGet]
        [Route("detail/{monthlyGoalId:Guid}")]
        public async Task<IActionResult> GetMonthlyGoalByIdAsync([FromRoute] Guid monthlyGoalId)
        {
            var monthlyGoal = await monthlyGoalService.GetMonthlyGoalByIdAsync(monthlyGoalId);
            if (monthlyGoal == null)
            {
                return NotFound(new ResponseObject
                {
                    Status = System.Net.HttpStatusCode.NotFound,
                    Message = "Monthly goal not found!",
                    Data = null
                });
            }

            return Ok(new ResponseObject
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = "Get monthly goal successfully!",
                Data = monthlyGoal
            });
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CreateAsync(MonthlyGoalRequest monthlyGoalRequest)
        {
            // Sử dụng helper để lấy UserId từ token
            var userId = JwtHelper.GetUserIdFromToken(HttpContext.Request, out var errorMessage);

            if (userId == null)
            {
                // Trả về lỗi nếu token không hợp lệ
                return Unauthorized(errorMessage);
            }

            var monthlyGoalResponse = await monthlyGoalService.AddMonthlyGoalAsync(userId.Value, monthlyGoalRequest);

            var response = new ResponseObject
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = "Create monthly goal successfully !",
                Data = monthlyGoalResponse
            };
            return Ok(response);
        }

        [HttpPut]
        [Route("{id:Guid}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] MonthlyGoalRequest monthlyGoalRequest)
        {
            // Sử dụng helper để lấy UserId từ token
            var userId = JwtHelper.GetUserIdFromToken(HttpContext.Request, out var errorMessage);

            if (userId == null)
            {
                // Trả về lỗi nếu token không hợp lệ
                return Unauthorized(errorMessage);
            }

            var monthlyGoalResponse = await monthlyGoalService.UpdateMonthlyGoalAsync(id, userId.Value, monthlyGoalRequest);
            if (monthlyGoalResponse == null)
            {
                return NotFound();
            }
            var response = new ResponseObject
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = "Update monthly goal successfully !",
                Data = monthlyGoalResponse
            };
            return Ok(response);
        }
    }
}
