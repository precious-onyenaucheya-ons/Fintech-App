using FluentEmail.Core;
using Microsoft.Extensions.Configuration;
using NotificationServices.Core.AppSettings;

namespace NotificationServicesAPI.Core.Utilities
{
    public class NotificationContext
    {
        public string Address { get; set; } = string.Empty;
        public string Header { get; set; } = string.Empty;
        public string Payload { get; set; } = null!;
        public NotificationSettings NotificationSettings { get; set; } = null!;
    }
}
