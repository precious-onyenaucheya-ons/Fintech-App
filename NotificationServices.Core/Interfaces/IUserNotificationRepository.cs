using NotificationServices.Domain.Models;
using NotificationServices.Infrastructure.Repository;

namespace NotificationServicesAPI.Core.Interfaces
{
    public interface IUserNotificationRepository : IGenericRepository<UserNotification>
    {
    }
}
