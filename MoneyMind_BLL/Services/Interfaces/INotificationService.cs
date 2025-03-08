using MoneyMind_BLL.DTOs.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace MoneyMind_BLL.Services.Interfaces
{
    public interface INotificationService
    {
        Task SendNotificationToUser(Guid userId, string message, string type = "info");
        Task SendNotificationToAll(string message);
        Task SendPushNotification(NotificationRequest request);
        Task SaveUserFcmToken(Guid userId, string fcmToken);
    }
}