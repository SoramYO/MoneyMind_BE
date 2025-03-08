using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using MoneyMind_BLL.DTOs.Chats;
using MoneyMind_BLL.DTOs.Messages;
using MoneyMind_BLL.Services.Interfaces;
using MoneyMind_DAL.Entities;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MoneyMind_API.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IChatService chatService;
        private readonly IMessageService messageService;

        public ChatHub(IChatService chatService, IMessageService messageService)
        {
            this.chatService = chatService;
            this.messageService = messageService;
        }

        public async Task SendMessage(Guid? chatId, Guid userId, string message)
        {
            // Kiểm tra tin nhắn không được rỗng
            if (string.IsNullOrWhiteSpace(message))
            {
                await Clients.Caller.SendAsync("ReceiveMessage", "ChatBot", "Tin nhắn không hợp lệ.");
                return;
            }

            // Nếu chatId không hợp lệ, tạo mới cuộc trò chuyện
            if (chatId == null || chatId == Guid.Empty)
            {
                var chatRequest = new ChatRequest
                {
                    LastMessageTime = DateTime.UtcNow,
                };
                var chatResponse = await chatService.AddChatAsync(userId, chatRequest);
                chatId = chatResponse.Id;
            }

            try
            {
                // Tạo đối tượng tin nhắn của người dùng
                var messageRequest = new MessageRequest
                {
                    SenderId = userId,
                    MessageContent = message,
                    MessageType = MessageType.Text,
                    IsBotResponse = false,
                    ChatId = chatId.Value,
                };

                // Lưu tin nhắn người dùng
                await messageService.AddMessageAsync(messageRequest);

                // Gọi IChatService để generate câu trả lời từ AI
                var botResponse = await chatService.GenerateResponseAsync(userId, message);

                // Tạo đối tượng tin nhắn của bot với nội dung trả lời từ AI
                var messageBotRequest = new MessageRequest
                {
                    SenderId = Guid.Empty, // hoặc sử dụng một Guid đại diện cho bot nếu có
                    MessageContent = botResponse,
                    MessageType = MessageType.Text,
                    IsBotResponse = true,
                    ChatId = chatId.Value,
                };

                // Lưu tin nhắn của bot
                var messageResponse = await messageService.AddMessageAsync(messageBotRequest);

                // Cập nhật lại thông tin cuộc trò chuyện (ví dụ: thời gian tin nhắn cuối)
                await chatService.UpdateChatAsync(userId, chatId.Value, new ChatRequest { LastMessageTime = messageResponse.SentTime });

                // Gửi câu trả lời của bot về cho client
                await Clients.Caller.SendAsync("ReceiveMessage", "ChatBot", messageResponse);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi (có thể ghi log chi tiết lỗi ở đây) và thông báo cho client
                await Clients.Caller.SendAsync("ReceiveMessage", "ChatBot", "Đã có lỗi xảy ra, vui lòng thử lại sau.");
            }
        }
    }
}
