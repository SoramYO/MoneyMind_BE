using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace MoneyMind_BLL.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(ILogger<ChatHub> logger)
        {
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("UserId is null or empty. ConnectionId: {ConnectionId}", Context.ConnectionId);
                throw new HubException("UserId is required.");
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
            _logger.LogInformation("User {UserId} connected to ChatHub", userId);
            await base.OnConnectedAsync();
        }

        public async Task SendMessage(string message)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return;

            // Echo back the message for now (we'll add AI integration later)
            await Clients.Caller.SendAsync("ReceiveMessage", new
            {
                message = message,
                isUser = true,
                timestamp = DateTime.UtcNow
            });
        }
    }
} 