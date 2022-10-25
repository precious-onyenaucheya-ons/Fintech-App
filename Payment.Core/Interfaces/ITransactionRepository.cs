using Payment.Domain.Models;
using System.Linq.Expressions;

namespace Payment.Core.Interfaces
{
    public interface ITransactionRepository : IGenericRepository<Transaction>
    {

        IQueryable<Transaction> GetTransactionHistory(string walletId);
        Task<Transaction> GetAsync(Expression<Func<Transaction, bool>> expression, List<string> includes = null);
        Task<Transaction> GetTransactionByReference(string reference);

    }
}
