using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MoneyMind_BLL.Services.Interfaces;
using System;
using System.Threading.Tasks;
using MoneyMind_BLL.DTOs.GoogleSheet;

namespace MoneyMind_API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SheetController : ControllerBase
	{
		private readonly ISheetService _sheetService;
		private readonly ILogger<SheetController> _logger;

		public SheetController(
			ISheetService sheetService,
			ILogger<SheetController> logger)
		{
			_sheetService = sheetService;
			_logger = logger;
		}

		[HttpPost]
		[Route("add")]
		public async Task<IActionResult> AddSheet([FromBody] AddSheetRequest request)
		{
			try
			{
				var result = await _sheetService.AddSheetAsync(request);
				return Ok(result);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error adding sheet");
				return StatusCode(500, new { Message = "Error adding sheet", Error = ex.Message });
			}
		}

		[HttpGet("user/{userId}")]
		public async Task<IActionResult> GetUserSheets(Guid userId)
		{
			try
			{
				var sheets = await _sheetService.GetUserSheetsAsync(userId);
				return Ok(sheets);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting user sheets");
				return StatusCode(500, new { Message = "Error getting user sheets", Error = ex.Message });
			}
		}
		[HttpGet("isExists/{userId}")]
		public async Task<IActionResult> IsSheetExists(Guid userId)
		{
			try
			{
				var sheets = await _sheetService.GetUserSheetsAsync(userId);
				var exists = sheets != null && sheets.Any();
				return Ok(new { Exists = exists });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error checking sheet existence");
				return StatusCode(500, new { Message = "Error checking sheet existence", Error = ex.Message });
			}
		}
	}
}
