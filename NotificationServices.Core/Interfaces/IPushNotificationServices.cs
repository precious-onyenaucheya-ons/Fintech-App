using NotificationServices.Core.DTOs;
using NotificationServicesAPI.Core.DTOs;

namespace NotificationServices.Core.Interfaces
{
    public interface IPushNotificationServices
    {
        Task<ResponseDto<string>> PushNotification(MessageDTO notification);
    }
}