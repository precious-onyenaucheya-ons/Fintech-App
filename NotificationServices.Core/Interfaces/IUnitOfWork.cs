namespace NotificationServicesAPI.Core.Interfaces
{
    public interface IUnitOfWork
    {
        INotificationRepository NotificationRepository { get; }
        IUserNotificationRepository UserNotificationRepository { get; }
        Task Commit();
        Task CreateTransaction();
        void Dispose();
        Task Rollback();
        Task Save();
    }
}
