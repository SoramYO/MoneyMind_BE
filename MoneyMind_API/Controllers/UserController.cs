using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyMind_BLL.DTOs;
using MoneyMind_BLL.Services.Interfaces;
using Libook_API.Configure;
using MoneyMind_BLL.DTOs.Users;
using System.Linq.Expressions;
using MoneyMind_DAL.Entities;
using MoneyMind_BLL.DTOs.MonthlyGoals;

namespace MoneyMind_API.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITransactionService _transactionService;
        private readonly IMonthlyGoalService _monthlyGoalService;

        public UserController(
            IUserService userService,
            ITransactionService transactionService,
            IMonthlyGoalService monthlyGoalService)
        {
            _userService = userService;
            _transactionService = transactionService;
            _monthlyGoalService = monthlyGoalService;
        }

        [HttpGet("balance")]
        public async Task<IActionResult> GetBalance()
        {
            var userId = JwtHelper.GetUserIdFromToken(HttpContext.Request, out var errorMessage);
            if (userId == null) return Unauthorized(errorMessage);

            var balance = await _userService.GetUserBalanceAsync(userId.Value);
            return Ok(new ResponseObject
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = "Get user balance successfully!",
                Data = new { Balance = balance }
            });
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = JwtHelper.GetUserIdFromToken(HttpContext.Request, out var errorMessage);
            if (userId == null) return Unauthorized(errorMessage);

            var profile = await _userService.GetUserProfileAsync(userId.Value);
            return Ok(new ResponseObject
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = "Get user profile successfully!",
                Data = profile
            });
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UserProfileRequest request)
        {
            var userId = JwtHelper.GetUserIdFromToken(HttpContext.Request, out var errorMessage);
            if (userId == null) return Unauthorized(errorMessage);

            var updatedProfile = await _userService.UpdateUserProfileAsync(userId.Value, request);
            return Ok(new ResponseObject
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = "Update user profile successfully!",
                Data = updatedProfile
            });
        }

        [HttpGet("transactions")]
        public async Task<IActionResult> GetTransactions(
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            var userId = JwtHelper.GetUserIdFromToken(HttpContext.Request, out var errorMessage);
            if (userId == null) return Unauthorized(errorMessage);

            Expression<Func<Transaction, bool>> filter = t => t.UserId == userId && t.IsActive;
            if (fromDate.HasValue)
                filter = filter.AndAlso(t => t.TransactionDate >= fromDate.Value);
            if (toDate.HasValue)
                filter = filter.AndAlso(t => t.TransactionDate <= toDate.Value);

            var transactions = await _transactionService.GetTransactionAsync(
                filter: filter,
                orderBy: q => q.OrderByDescending(t => t.TransactionDate),
                includeProperties: "TransactionTags.Tag",
                pageIndex: pageIndex,
                pageSize: pageSize);

            return Ok(new ResponseObject
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = "Get user transactions successfully!",
                Data = transactions
            });
        }

        [HttpGet("transactions/category")]
        public async Task<IActionResult> GetTransactionsByCategory(
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            var userId = JwtHelper.GetUserIdFromToken(HttpContext.Request, out var errorMessage);
            if (userId == null) return Unauthorized(errorMessage);

            var categoryStats = await _userService.GetTransactionsByCategoryAsync(userId.Value, fromDate, toDate);
            return Ok(new ResponseObject
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = "Get transactions by category successfully!",
                Data = categoryStats
            });
        }

        [HttpPost("monthgoal")]
        public async Task<IActionResult> CreateMonthlyGoal([FromBody] MonthlyGoalRequest request)
        {
            var userId = JwtHelper.GetUserIdFromToken(HttpContext.Request, out var errorMessage);
            if (userId == null) return Unauthorized(errorMessage);

            var monthlyGoal = await _monthlyGoalService.AddMonthlyGoalAsync(userId.Value, request);
            return Ok(new ResponseObject
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = "Create monthly goal successfully!",
                Data = monthlyGoal
            });
        }
    }
} 