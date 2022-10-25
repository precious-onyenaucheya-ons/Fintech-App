using NotificationServices.Domain.Models;
using NotificationServicesAPI.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationServices.Infrastructure.Repository
{
    public class NotificationRepository: GenericRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(NotificationDbContext context) : base(context)
        {
        }
    }
}
