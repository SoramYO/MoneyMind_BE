using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyMind_BLL.Services.Interfaces;
using MoneyMind_DAL.Repositories.Interfaces;

namespace MoneyMind_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MBBankSyncController : ControllerBase
    {
        private readonly IMBBankSyncService _mbBankSyncService;
        private readonly IAccountBankRepository _accountBankRepository;
        private readonly ILogger<MBBankSyncController> _logger;

        public MBBankSyncController(
            IMBBankSyncService mbBankSyncService,
            IAccountBankRepository accountBankRepository,
            ILogger<MBBankSyncController> logger)
        {
            _mbBankSyncService = mbBankSyncService;
            _accountBankRepository = accountBankRepository;
            _logger = logger;
        }

        [HttpPost("sync")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SyncTransactions()
        {
            try
            {
                var accounts = await _accountBankRepository.GetAllUserIds();
                var syncResults = new List<object>();

                foreach (var account in accounts)
                {
                    try
                    {
                        await _mbBankSyncService.SyncTransactions(account.UserId);
                        syncResults.Add(new
                        {
                            UserId = account.UserId,
                            Status = "Success"
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error syncing transactions for user {UserId}", account.UserId);
                        syncResults.Add(new
                        {
                            UserId = account.UserId,
                            Status = "Failed",
                            Error = ex.Message
                        });
                    }
                }

                return Ok(new
                {
                    Message = "Sync process completed",
                    Results = syncResults
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in manual sync process");
                return StatusCode(500, new { Message = "Internal server error during sync process", Error = ex.Message });
            }
        }

        [HttpPost("sync/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SyncTransactionsForUser(Guid userId)
        {
            try
            {
                await _mbBankSyncService.SyncTransactions(userId);
                return Ok(new { Message = $"Successfully synced transactions for user {userId}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error syncing transactions for user {UserId}", userId);
                return StatusCode(500, new { Message = "Error syncing transactions", Error = ex.Message });
            }
        }
    }
}