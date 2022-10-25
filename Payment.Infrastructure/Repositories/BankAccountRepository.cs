using Microsoft.EntityFrameworkCore;
using Payment.Core.Interfaces;
using Payment.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Infrastructure.Repositories
{
    public class BankAccountRepository : GenericRepository<BankAccount>, IBankAccountRepository
    {
        private readonly PaymentDbContext context;
        public BankAccountRepository(PaymentDbContext dbContext) : base(dbContext)
        {
            context = dbContext;
        }

        public IQueryable<BankAccount> GetAllBeneficiaryAsync(string id)
        {
            return context.BankAccounts.Where(x => x.WalletId == id).Include(b => b.Bank);
        }
        public async Task<BankAccount> GetABeneficiary(string accountnumber)
        {       
            return await context.BankAccounts.FirstOrDefaultAsync(x => x.AccountNumber == accountnumber);

        }

        public async Task<BankAccount> GetBeneficiaryById(string id, string AccountNumber)
        {
            return await context.BankAccounts.Include(b => b.Bank).FirstOrDefaultAsync(x => x.WalletId == id && x.AccountNumber == AccountNumber);
        }
    }
}
