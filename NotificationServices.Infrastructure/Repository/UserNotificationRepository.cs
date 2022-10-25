using NotificationServices.Domain.Models;
using NotificationServicesAPI.Core.Interfaces;

namespace NotificationServices.Infrastructure.Repository
{
    public class UserNotificationRepository : GenericRepository<UserNotification>, IUserNotificationRepository
    {
        public UserNotificationRepository(NotificationDbContext context): base(context)
        {
        }
    }
}
