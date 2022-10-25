using Microsoft.AspNetCore.Identity.UI.Services;
using NotificationServices.Core.DTOs;
using NotificationServices.Domain.Enums;
using NotificationServicesAPI.Core.DTOs;
using NotificationServicesAPI.Core.Interfaces;
using NotificationServicesAPI.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NotificationServices.Core.Services
{
    public class EmailService : IEmailService
    {
        private readonly INotificationService _notificationService;

        public EmailService(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task<ResponseDto<bool>> SendEmail(EmailDTO model)
        {
            var context = new NotificationContext
            {
                Header = model.Subject,
                Address = model.ToEmail,
                Payload = model.Message
            };
            var result = await _notificationService.SendAsync(NotificationType.Email, context);
            if (!result)
                return ResponseDto<bool>.Fail("Could not send email", (int) HttpStatusCode.ServiceUnavailable);
            
            return ResponseDto<bool>.Success("Successfully sent email", true, (int)HttpStatusCode.OK);
        }
    }
}
