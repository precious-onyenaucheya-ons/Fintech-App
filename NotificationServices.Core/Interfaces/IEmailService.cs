using NotificationServices.Core.DTOs;
using NotificationServicesAPI.Core.DTOs;
using NotificationServicesAPI.Core.Utilities;

namespace NotificationServicesAPI.Core.Interfaces
{
    public interface IEmailService
    {
        Task<ResponseDto<bool>> SendEmail(EmailDTO dTO);
    }
}
