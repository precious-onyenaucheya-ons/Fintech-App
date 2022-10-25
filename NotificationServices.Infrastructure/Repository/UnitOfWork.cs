using Microsoft.EntityFrameworkCore.Storage;
using NotificationServices.Core.Interfaces;
using NotificationServicesAPI.Core.Interfaces;

namespace NotificationServices.Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private bool _disposedValue;
        private readonly NotificationDbContext _context;
        private IDbContextTransaction _objTransaction;
        INotificationRepository _notificationRepository;
        IUserNotificationRepository _userNotificationRepository;
        public UnitOfWork(NotificationDbContext context)
        {
            _context = context;
        }
        public INotificationRepository NotificationRepository => _notificationRepository ??= new NotificationRepository(_context);
        public IUserNotificationRepository UserNotificationRepository => _userNotificationRepository ??= new UserNotificationRepository(_context);
        public async Task CreateTransaction()
        {
            _objTransaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task Commit()
        {
            await _objTransaction.CommitAsync();
        }

        public async Task Rollback()
        {
            await _objTransaction.RollbackAsync();
            await _objTransaction.DisposeAsync();
        }

        public async Task Save()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {
                throw new Exception();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
