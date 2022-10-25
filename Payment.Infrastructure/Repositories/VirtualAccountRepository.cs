using Microsoft.EntityFrameworkCore;
using Payment.Core.Interfaces;
using Payment.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Infrastructure.Repositories
{
    public class VirtualAccountRepository : GenericRepository<VirtualAccount>, IVirtualAccountRepository
    {
        private readonly PaymentDbContext _dbContext;
        public VirtualAccountRepository(PaymentDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<VirtualAccount> GetVirtualAccountByAccountNumber(string accountNumber)
        {
            return await _dbContext.VirtualAccounts
                .Include(v => v.Wallet)
                .SingleOrDefaultAsync(v => v.AccountNumber == accountNumber) ?? new VirtualAccount();
        }
    }
}
