using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using NotificationServices.Core.DTOs;
using NotificationServices.Core.Interfaces;
using NotificationServicesAPI.Core.Interfaces;

namespace NotificationServicesAPI.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly IHubContext<MessageHub> _messageHub;
        private readonly IPushNotificationServices _pushNotification;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="emailService"></param>
        public NotificationController(IEmailService emailService, IHubContext<MessageHub> messageHub, IPushNotificationServices pushNotification)
        {
            _emailService = emailService;
            _messageHub = messageHub;
            _pushNotification = pushNotification;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost("send-email")]
        public async Task<IActionResult> SendMail(EmailDTO model)
        {
            var result = await _emailService.SendEmail(model);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPost("public-notification")]
        public async Task<IActionResult> PublicNotification(MessageDTO notification)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = await _pushNotification.PushNotification(notification);
            if (result.Status)
            {
                await _messageHub.Clients.All.SendAsync("Recieve", $"The message {notification.PublicMessage} has been received.");
                return Ok();
            }
            return BadRequest();
        }
    }
}