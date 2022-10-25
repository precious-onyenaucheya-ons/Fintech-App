using NotificationServices.Core.DTOs;
using NotificationServices.Core.Interfaces;
using NotificationServices.Domain.Enums;
using NotificationServices.Domain.Models;
using NotificationServicesAPI.Core.DTOs;
using NotificationServicesAPI.Core.Interfaces;
using System.Net;

namespace NotificationServices.Core.Services
{
    public class PushNotificationServices : IPushNotificationServices
    {
        private readonly IUnitOfWork _unitOfWork;
        public PushNotificationServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ResponseDto<string>> PushNotification(MessageDTO notification)
        {
            try
            {
                var addNotification = new Notification()
                {
                    Message = notification.PublicMessage,
                    Title = "All",
                    NotificationType = NotificationType.PushNotification
                };
                await _unitOfWork.NotificationRepository.InsertAsync(addNotification);
                var addUserNotification = new UserNotification()
                {
                    UserId = notification.UserId,
                    NotificationId = addNotification.Id
                };
                await _unitOfWork.UserNotificationRepository.InsertAsync(addUserNotification);
                await _unitOfWork.Save();
                return ResponseDto<string>.Success("Saved Successful", "Notification", (int)HttpStatusCode.OK);
            }
            catch (Exception)     
            {
                return ResponseDto<string>.Fail("Not Saved", (int)HttpStatusCode.InternalServerError);
            }
          
        }
    }
}
