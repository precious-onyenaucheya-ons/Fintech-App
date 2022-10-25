 using Payment.Core.Interfaces;

namespace Payment.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PaymentDbContext _context;
        private BankAccountRepository _bankAccounts { get; set; } = null!;
        private BankRepository _banks { get; set; } = null!;
        private TransactionRepository _transactions { get; set; } = null!;
        private VirtualAccountRepository _virtualAccounts { get; set; } = null!;
        private WalletRepository _wallets { get; set; } = null!;
        public UnitOfWork(PaymentDbContext context)
        {
            _context = context;
        }
        public IBankAccountRepository BankAccounts => _bankAccounts ??= new BankAccountRepository(_context);
        public ITransactionRepository Transactions => _transactions ??= new TransactionRepository(_context);

        public IVirtualAccountRepository VirtualAccounts => _virtualAccounts ??= new VirtualAccountRepository(_context);

        public IWalletRepository Wallets => _wallets ??= new WalletRepository(_context);

        public IBankRepository Banks => _banks ??= new BankRepository(_context);

        public ITransactionRepository TransactionRepository => throw new NotImplementedException();

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
        public async Task Rollback()
        {
            await _context.Database.RollbackTransactionAsync();
        }
    }
}
