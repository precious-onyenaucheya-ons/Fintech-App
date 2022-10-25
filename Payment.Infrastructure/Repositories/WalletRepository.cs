using Microsoft.EntityFrameworkCore;
using Payment.Core.Interfaces;
using Payment.Domain.Models;

namespace Payment.Infrastructure.Repositories
{
    public class WalletRepository : GenericRepository<Wallet>, IWalletRepository
    {
        private readonly PaymentDbContext _dbContext;

        public WalletRepository(PaymentDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Gets wallet by id or code from the database
        /// </summary>
        /// <param name="id">ID or user's email address</param>
        /// <returns>Returns wallet if found and null if not found</returns>
        public async Task<Wallet?> Get(string id)
        {
            return await _dbContext.Wallets.Include(w => w.VirtualAccount).ThenInclude(v => v.Bank).FirstOrDefaultAsync(w => w.Id == id || w.Code == id);
        }

        /// <summary>
        /// Gets a wallet with matching UserId property from the database
        /// </summary>
        /// <param name="userId">ID of user whcih the wallet belongs to</param>
        /// <returns>Returns the user's wallet if found and null if not found</returns>
        public async Task<Wallet?> GetUserWallet(string userId)
        {
            return await _dbContext.Wallets.Include(w => w.VirtualAccount).ThenInclude(v => v.Bank).FirstOrDefaultAsync(w => w.UserId == userId);
        }

    }
}
