using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationServices.Core.AppSettings;
using NotificationServices.Core.Utilities;
using NotificationServices.Domain.Enums;
using NotificationServicesAPI.Core.Interfaces;
using NotificationServicesAPI.Core.Utilities;
using ILogger = Serilog.ILogger;

namespace NotificationServices.Infrastructure.ExternalServices
{
    public class NotificationService : INotificationService
    {
        private readonly ILogger _logger;
        private readonly NotificationSettings _notificationSettings;
        private readonly Dictionary<NotificationType, INotificationProvider>
            _notificationProviders = new Dictionary<NotificationType, INotificationProvider>();

        public NotificationService(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger>();
            _notificationSettings = serviceProvider.GetRequiredService<NotificationSettings>();
            _notificationProviders.Add(NotificationType.Email, new EmailNotificationProvider(serviceProvider));
        }
        
        public async Task<bool> SendAsync(NotificationType type, NotificationContext context)
        {
            context.NotificationSettings = _notificationSettings;
            try
            {
                var response = !type.HasFlag(NotificationType.Email) ||
                    await _notificationProviders[NotificationType.Email].SendAsync(context);

                if(!response)
                {
                    return await Task.FromResult(false);
                }
            }
            catch (Exception)
            {
                _logger.Error($"notification Error: {context.Address} => {context.Header}");

                return await Task.FromResult(false);
            }
            
            return await Task.FromResult(true);
        }
    }
}