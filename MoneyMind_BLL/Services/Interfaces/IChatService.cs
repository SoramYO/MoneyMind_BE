using MoneyMind_BLL.DTOs.Chats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_BLL.Services.Interfaces
{
    public interface IChatService
    {
        Task<ChatResponse> AddChatAsync(Guid userId, ChatRequest chatRequest);
        Task<ChatResponse> GetChatByUserIdAsync(Guid userId);
        Task<ChatResponse> GetChatByIdAsync(Guid chatId);
        Task<ChatResponse> UpdateChatAsync(Guid userId, Guid chatId, ChatRequest chatRequest);
        Task<string> GenerateResponseAsync(Guid userId, string message);
    }
}
