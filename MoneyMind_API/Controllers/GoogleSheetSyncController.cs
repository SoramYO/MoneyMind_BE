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
        private readonly INotificationService _notificationService;

        public GoogleSheetSyncController(
            IGoogleSheetSyncService googleSheetSyncService,
            ISheetService sheetService,
            INotificationService notificationService,
            ILogger<GoogleSheetSyncController> logger)
        {
            _googleSheetSyncService = googleSheetSyncService;
            _sheetService = sheetService;
            _notificationService = notificationService;
            _logger = logger;
        }

        [HttpPost("sync/{userId}")]
        public async Task<IActionResult> SyncTransactions(Guid userId)
        {
            try
            {
                var userSheets = await _sheetService.GetUserSheetsAsync(userId);
                
                if (!userSheets.Any())
                {
                    await _notificationService.SendNotificationToUser(
                        userId,
                        "Không tìm thấy Google Sheet nào để đồng bộ",
                        "warning"
                    );
                    return NotFound(new { Message = "No sheets found for this user" });
                }

                var totalNewTransactions = 0;
                var successfulSheets = 0;
                var failedSheets = 0;

                foreach (var sheet in userSheets)
                {
                    try
                    {
                        var request = new GoogleSheetRequest
                        {
                            SheetId = sheet.SheetId,
                            UserId = userId
                        };

                        var result = await _googleSheetSyncService.SyncTransactionsFromSheet(request);
                        totalNewTransactions += result.NewTransactions;
                        successfulSheets++;
                    }
                    catch (Exception ex)
                    {
                        failedSheets++;
                        _logger.LogError(ex, "Error syncing sheet {SheetId}", sheet.SheetId);
                    }
                }

                // Gửi thông báo kết quả đồng bộ
                var message = $"Đồng bộ hoàn tất: {successfulSheets} sheet thành công, {failedSheets} sheet thất bại. " +
                            $"Đã thêm {totalNewTransactions} giao dịch mới.";
                var type = failedSheets > 0 ? "warning" : "success";
                
                await _notificationService.SendNotificationToUser(
                    userId,
                    message,
                    type
                );

                return Ok(new { 
                    Message = "Sync completed",
                    NewTransactions = totalNewTransactions,
                    SuccessfulSheets = successfulSheets,
                    FailedSheets = failedSheets
                });
            }
            catch (Exception ex)
            {
                await _notificationService.SendNotificationToUser(
                    userId,
                    "Đồng bộ thất bại: " + ex.Message,
                    "error"
                );
                return StatusCode(500, new { Message = "Error during sync process", Error = ex.Message });
            }
        }
    }
} 