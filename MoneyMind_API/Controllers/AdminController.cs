using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyMind_BLL.DTOs.Admin;
using MoneyMind_BLL.Services.Interfaces;
using MoneyMind_DAL.Entities;
using System.Linq.Expressions;

namespace MoneyMind_API.Controllers
{
    [Route("api/admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            IAdminService adminService,
            ILogger<AdminController> logger)
        {
            _adminService = adminService;
            _logger = logger;
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetAllUsers([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _adminService.GetAllUsersAsync(pageIndex, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                return StatusCode(500, new { Message = "Error getting all users", Error = ex.Message });
            }
        }

        [HttpPost("user")]
        public async Task<IActionResult> CreateUser([FromBody] AdminCreateUserRequest request)
        {
            try
            {
                var result = await _adminService.CreateUserAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                return StatusCode(500, new { Message = "Error creating user", Error = ex.Message });
            }
        }

        [HttpPut("user/{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] AdminUpdateUserRequest request)
        {
            try
            {
                var result = await _adminService.UpdateUserAsync(id, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user");
                return StatusCode(500, new { Message = "Error updating user", Error = ex.Message });
            }
        }

        [HttpDelete("user/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                var result = await _adminService.DeleteUserAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user");
                return StatusCode(500, new { Message = "Error deleting user", Error = ex.Message });
            }
        }

        [HttpGet("reports")]
        public async Task<IActionResult> GetReports()
        {
            try
            {
                var result = await _adminService.GetReportsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting reports");
                return StatusCode(500, new { Message = "Error getting reports", Error = ex.Message });
            }
        }

        [HttpGet("transactions")]
        public async Task<IActionResult> GetAllTransactions(
            [FromQuery] string? category,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                Expression<Func<Transaction, bool>>? filter = null;
                if (!string.IsNullOrEmpty(category) || fromDate.HasValue || toDate.HasValue)
                {
                    filter = t =>
                        (string.IsNullOrEmpty(category) || t.TransactionTags.Any(tt => tt.Tag.Name == category)) &&
                        (!fromDate.HasValue || t.TransactionDate >= fromDate) &&
                        (!toDate.HasValue || t.TransactionDate <= toDate);
                }

                var result = await _adminService.GetAllTransactionsAsync(
                    filter: filter,
                    orderBy: q => q.OrderByDescending(t => t.TransactionDate),
                    includeProperties: "Tag",
                    pageIndex: pageIndex,
                    pageSize: pageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all transactions");
                return StatusCode(500, new { Message = "Error getting all transactions", Error = ex.Message });
            }
        }
    }
} 