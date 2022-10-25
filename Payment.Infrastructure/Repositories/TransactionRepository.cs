using Microsoft.EntityFrameworkCore;
using Payment.Core.Interfaces;
using Payment.Domain.Models;
using System.Linq.Expressions;

namespace Payment.Infrastructure.Repositories
{
    public class TransactionRepository : GenericRepository<Transaction>, ITransactionRepository
    {
        private readonly PaymentDbContext _dbContext;

        public TransactionRepository(PaymentDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Gets All the transactions that relate to a particular wallet
        /// </summary>
        /// <param name="WalletId"></param>
        /// <returns>IQuryable of Transactions related to the given wallet Id</returns>
        public IQueryable<Transaction> GetTransactionHistory(string walletId)
        {
            return _dbContext.Transactions.Where(w => w.WalletId == walletId).Select(t => t);
        }

        public async Task<Transaction> GetTransactionByReference(string reference)
        {
            return await _dbContext.Transactions.Where(x => x.Reference == reference).SingleOrDefaultAsync();
        }
        


        public async Task<Transaction> GetAsync(Expression<Func<Transaction, bool>> expression, List<string> includes = null)
        {
            var query = _dbContext.Transactions.Where(expression);
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            return await query.FirstOrDefaultAsync(expression);
        }


    }
}
