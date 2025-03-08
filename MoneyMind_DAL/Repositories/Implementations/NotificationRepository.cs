using MoneyMind_DAL.DBContexts;
using MoneyMind_DAL.Entities;
using MoneyMind_DAL.Repositories.Interfaces;

namespace MoneyMind_DAL.Repositories.Implementations
{
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(MoneyMindDbContext context) : base(context)
        {
        }
    }
} 