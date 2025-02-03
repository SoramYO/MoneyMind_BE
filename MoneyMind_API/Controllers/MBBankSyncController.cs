using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoneyMind_BLL.DTOs.Accounts;
using MoneyMind_BLL.Services.Interfaces;
using MoneyMind_DAL.Repositories.Implementations;
using MoneyMind_DAL.Repositories.Interfaces;

namespace MoneyMind_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MBBankSyncController : ControllerBase
    {
        private readonly IMBBankSyncService mbBankSyncService;
        private readonly IAccountBankService accountBankService;
        private readonly UserManager<IdentityUser> userManager;
        private readonly ILogger<MBBankSyncController> logger;

        public MBBankSyncController(
            IMBBankSyncService mbBankSyncService,
            IAccountBankService accountBankService,
            UserManager<IdentityUser> userManager,
            ILogger<MBBankSyncController> logger)
        {
            this.mbBankSyncService = mbBankSyncService;
            this.accountBankService = accountBankService;
            this.userManager = userManager;
            this.logger = logger;
        }

        [HttpGet]
        [Route("sync")]
        public async Task<IActionResult> SyncTransactions()
        {
            try
            {
                var users = userManager.Users.ToList();
                var syncResults = new List<object>();

                foreach (var user in users)
                {
                    try
                    {
                        // Kiem tra xem nguoi d�ng c� t�i khoan ng�n h�ng kh�ng
                        var accountBank = await accountBankService.GetAccoutBankByUserIdAsync(Guid.Parse(user.Id));
                        if (accountBank != null && accountBank.Any())
                        {
                            // dong bo giao dich
                            await mbBankSyncService.SyncTransactions(Guid.Parse(user.Id));
                            syncResults.Add(new
                            {
                                UserId = user.Id,
                                UserName = user.UserName,
                                Status = "Success"
                            });
                        }
                        else
                        {
                            syncResults.Add(new
                            {
                                UserId = user.Id,
                                UserName = user.UserName,
                                Status = "Skipped",
                                Message = "No bank account linked"
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error syncing transactions for user {UserId}", user.Id);
                        syncResults.Add(new
                        {
                            UserId = user.Id,
                            UserName = user.UserName,
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
                logger.LogError(ex, "Error in manual sync process");
                return StatusCode(500, new { Message = "Internal server error during sync process", Error = ex.Message });
            }
        }

        [HttpPost]
        [Route("sync/{userId}")]
        public async Task<IActionResult> SyncTransactionsForUser(string userId)
        {
            try
            {
                var accountBanks = await accountBankService.GetAccoutBankByUserIdAsync(Guid.Parse(userId));

                if (accountBanks == null || !accountBanks.Any())
                {
                    logger.LogWarning("No bank accounts found for user {UserId}", userId);
                    return NotFound(new { Message = $"No bank accounts found for user {userId}" });
                }

                await mbBankSyncService.SyncTransactions(Guid.Parse(userId));
                logger.LogInformation("Successfully synced transactions for user {UserId} at {Time}", userId, DateTimeOffset.Now);

                return Ok(new { Message = $"Successfully synced transactions for user {userId}" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error syncing transactions for user {UserId} at {Time}", userId, DateTimeOffset.Now);
                return StatusCode(500, new
                {
                    Message = $"Error syncing transactions for user {userId}",
                    Error = ex.Message
                });
            }
        }

    }
}