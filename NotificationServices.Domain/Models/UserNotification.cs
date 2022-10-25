using System.ComponentModel.DataAnnotations.Schema;

namespace NotificationServices.Domain.Models
{
    public class UserNotification
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; }
        public string NotificationId { get; set; }
        public ICollection<Notification> Notifications { get; set; }
        public UserNotification()
        {
            Notifications = new HashSet<Notification>();
        }
    }
}
