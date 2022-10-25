using NotificationServices.Domain.Enums;
using NotificationServicesAPI.Core.Utilities;

namespace NotificationServicesAPI.Core.Interfaces
{
    public interface INotificationService
    {
        Task<bool> SendAsync(NotificationType type, NotificationContext context);
    }
}
