using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoneyMind_BLL.DTOs;
using MoneyMind_BLL.Services.Interfaces;

namespace MoneyMind_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService chatService;

        public ChatController(IChatService chatService)
        {
            this.chatService = chatService;
        }

        [HttpGet]
        [Route("{userId:Guid}")]
        [Authorize]
        public async Task<IActionResult> GetChatByUserIdAsync([FromRoute] Guid userId)
        {
            var chat = await chatService.GetChatByUserIdAsync(userId);
            if (chat == null)
            {
                return NotFound();
            }
            var response = new ResponseObject
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = "Get chat successfully!",
                Data = chat
            };
            return Ok(response);
        }

        [HttpGet]
        [Route("detail/{chatId:Guid}")]
        [Authorize]
        public async Task<IActionResult> GetChatByIdAsync([FromRoute] Guid chatId)
        {
            var chat = await chatService.GetChatByIdAsync(chatId);
            if (chat == null)
            {
                return NotFound();
            }
            var response = new ResponseObject
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = "Get chat successfully!",
                Data = chat
            };
            return Ok(response);
        }

        [HttpGet]
        [Route("airesponse")]
        public async Task<IActionResult> GetAiResponseAsync([FromQuery]  string message)
        {
            // Sử dụng helper để lấy UserId từ token
            var userId = JwtHelper.GetUserIdFromToken(HttpContext.Request, out var errorMessage);

            if (userId == null)
            {
                // Trả về lỗi nếu token không hợp lệ
                return Unauthorized(errorMessage);
            }

            var chat = await chatService.GenerateResponseAsync(userId.Value, message);
            if (chat == null)
            {
                return NotFound();
            }
            var response = new ResponseObject
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = "Get chat successfully!",
                Data = chat
            };
            return Ok(response);
        }
    }
}
