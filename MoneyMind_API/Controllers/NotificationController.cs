using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyMind_BLL.DTOs;
using MoneyMind_BLL.DTOs.Notification;
using MoneyMind_BLL.Services.Interfaces;


namespace MoneyMind_API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class NotificationController : ControllerBase
	{
		private readonly INotificationService _notificationService;

		public NotificationController(INotificationService notificationService)
		{
			_notificationService = notificationService;
		}
		[HttpPost("send-notification")]
		public async Task<IActionResult> SendNotification([FromBody] string message)
		{
			var userId = JwtHelper.GetUserIdFromToken(HttpContext.Request, out var errorMessage);
			if (userId == null)
			{
				return Unauthorized(errorMessage);
			}

			await _notificationService.SendNotificationToUser(
				userId.Value,
				message,
				"info"
			);

			return Ok(new ResponseObject
			{
				Status = System.Net.HttpStatusCode.OK,
				Message = "Notification sent successfully!"
			});
		}


	}
}