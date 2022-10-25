using NotificationServices.Domain.Models;
using NotificationServices.Infrastructure.Repository;

namespace NotificationServicesAPI.Core.Interfaces
{
    public interface INotificationRepository : IGenericRepository<Notification>
    {
    }
}
