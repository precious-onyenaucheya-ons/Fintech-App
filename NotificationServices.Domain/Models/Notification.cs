using NotificationServices.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace NotificationServices.Domain.Models
{
    public class Notification
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public NotificationType NotificationType { get; set; } 
        public UserNotification UserNotification { get; set; }
    }
}
