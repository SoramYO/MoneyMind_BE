using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using MoneyMind_BLL.DTOs.Notification;
using MoneyMind_BLL.Hubs;
using MoneyMind_BLL.Services.Interfaces;
using MoneyMind_DAL.Entities;
using MoneyMind_DAL.Repositories.Interfaces;


public class NotificationService : INotificationService
{
    private readonly FirebaseMessaging _firebaseMessaging;
    private readonly IUserFcmTokenRepository _fcmTokenRepository;

	public NotificationService(
		IUserFcmTokenRepository fcmTokenRepository)
    {
        _fcmTokenRepository = fcmTokenRepository;

        if (FirebaseApp.DefaultInstance == null)
        {
            var projectDir = AppDomain.CurrentDomain.BaseDirectory;
            var credentialPath = Path.Combine(projectDir, "firebase-adminsdk.json");
            
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile(credentialPath)
            });
        }
        _firebaseMessaging = FirebaseMessaging.DefaultInstance;
    }

    public async Task SendNotificationToUser(Guid userId, string message, string type = "info")
    {
        try
        {
            var userTokens = await _fcmTokenRepository.GetAsync(
                filter: t => t.UserId == userId
            );

            if (!userTokens.Item1.Any())
            {
				Console.WriteLine("No FCM token found for user {UserId}", userId);
                return;
            }

            foreach (var token in userTokens.Item1)
            {
                await SendPushNotification(new NotificationRequest
                {
                    Title = "MoneyMind Notification",
                    Body = message,
                    FcmToken = token.FcmToken,
                    Data = new Dictionary<string, string> { { "type", type } }
                });
            }
        }
        catch (Exception ex)
        {
			Console.WriteLine("Error sending notification to user {UserId}", userId);
        }
    }

    public async Task SendNotificationToAll(string message)
    {
        try
        {
            var tokens = await _fcmTokenRepository.GetAll();
            foreach (var token in tokens)
            {
                await SendPushNotification(new NotificationRequest
                {
                    Title = "MoneyMind Broadcast",
                    Body = message,
                    FcmToken = token.FcmToken
                });
            }
        }
        catch (Exception ex)
        {
			Console.WriteLine("Error broadcasting notification");
        }
    }

    public async Task SaveUserFcmToken(Guid userId, string fcmToken)
    {
        var token = new UserFcmToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            FcmToken = fcmToken,
            CreatedAt = DateTime.UtcNow,
            LastUsed = DateTime.UtcNow
        };

        await _fcmTokenRepository.InsertAsync(token);
    }

    public async Task SendPushNotification(NotificationRequest request)
    {
        try
        {
            var message = new FirebaseAdmin.Messaging.Message()
            {
                Token = request.FcmToken,
                Notification = new FirebaseAdmin.Messaging.Notification
                {
                    Title = request.Title,
                    Body = request.Body
                },
                Data = request.Data,
                Android = new AndroidConfig
                {
                    Priority = Priority.High,
                    Notification = new AndroidNotification
                    {
                        Sound = "default",
                        Priority = NotificationPriority.HIGH
                    }
                },
                Apns = new ApnsConfig
                {
                    Aps = new Aps
                    {
                        Sound = "default",
                        Badge = 1
                    }
                }
            };

            var result = await _firebaseMessaging.SendAsync(message);
            Console.WriteLine("FCM notification sent successfully: {MessageId}", result);
        }
        catch (Exception ex)
        {
			Console.WriteLine( "Error sending FCM notification");
            throw;
        }
    }
} 