using NotificationServicesAPI.Core.Utilities;

namespace NotificationServicesAPI.Core.Interfaces
{
    public interface INotificationProvider
    {
        Task<bool> SendAsync(NotificationContext context);
    }
}
