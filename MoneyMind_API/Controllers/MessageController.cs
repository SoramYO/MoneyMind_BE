using Libook_API.Configure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoneyMind_BLL.DTOs;
using MoneyMind_BLL.Services.Implementations;
using MoneyMind_BLL.Services.Interfaces;
using MoneyMind_DAL.Entities;
using System.Linq.Expressions;

namespace MoneyMind_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService messageService;

        public MessageController(IMessageService messageService)
        {
            this.messageService = messageService;
        }

        [HttpGet]
        [Route("{userId:Guid}")]
        [Authorize]
        public async Task<IActionResult> GetAsync(
             [FromRoute] Guid userId,
             [FromQuery] Guid chatId,
             [FromQuery] int pageIndex = 1,
             [FromQuery] int pageSize = 20)
        {
            // Đảm bảo pageIndex và pageSize hợp lệ
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize < 1) pageSize = 20;

            // Tạo bộ lọc
            Expression<Func<Message, bool>> filterExpression = s => s.Chat.UserId == userId;

            filterExpression = filterExpression.AndAlso(s => s.ChatId == chatId);

            // Tạo sắp xếp linh hoạt
            Func<IQueryable<Message>, IOrderedQueryable<Message>> orderByFunc = q => q.OrderByDescending(m => m.SentTime);

            // Lấy dữ liệu
            var listDataResponse = await messageService.GetMessageByChatIdAsync(
                filter: filterExpression,
                orderBy: orderByFunc,
                includeProperties: "",
                pageIndex: pageIndex,
                pageSize: pageSize
            );

            var response = new ResponseObject
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = "Get message of user successfully!",
                Data = listDataResponse
            };

            return Ok(response);
        }
    }
}
