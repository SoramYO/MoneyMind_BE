using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MoneyMind_BLL.DTOs.GoogleSheet;
using MoneyMind_BLL.Services.Interfaces;
using MoneyMind_DAL.Entities;

namespace MoneyMind_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoogleSheetSyncController : ControllerBase
    {
        private readonly IGoogleSheetSyncService _googleSheetSyncService;
        private readonly ISheetService _sheetService;
        private readonly ILogger<GoogleSheetSyncController> _logger;

        public GoogleSheetSyncController(
            IGoogleSheetSyncService googleSheetSyncService,
            ISheetService sheetService,
            ILogger<GoogleSheetSyncController> logger)
        {
            _googleSheetSyncService = googleSheetSyncService;
            _sheetService = sheetService;
            _logger = logger;
        }

        [HttpPost("sync/{userId}")]
        public async Task<IActionResult> SyncTransactions(Guid userId)
        {
            try
            {
                // Get all sheets associated with the user
                var userSheets = await _sheetService.GetUserSheetsAsync(userId);
                
                if (!userSheets.Any())
                {
                    return NotFound(new { Message = "No sheets found for this user" });
                }

                var syncResults = new List<object>();

                foreach (var sheet in userSheets)
                {
                    try
                    {
                        var request = new GoogleSheetRequest
                        {
                            SheetId = sheet.SheetId,
                            UserId = userId
                        };

                        await _googleSheetSyncService.SyncTransactionsFromSheet(request);
                        
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error syncing sheet {SheetId}", sheet.SheetId);
                        syncResults.Add(new
                        {
                            SheetId = sheet.SheetId,
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
                _logger.LogError(ex, "Error syncing transactions for user {UserId}", userId);
                return StatusCode(500, new { Message = "Error syncing transactions", Error = ex.Message });
            }
        }
    }
} 