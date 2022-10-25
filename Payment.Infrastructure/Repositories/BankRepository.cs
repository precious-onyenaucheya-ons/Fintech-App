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
    public class BankRepository : GenericRepository<Bank>, IBankRepository
    {
        private readonly PaymentDbContext _context;

        public BankRepository(PaymentDbContext dbContext) : base(dbContext)
        {
            _context = dbContext;
        }

        public async Task<Bank> GetBank(Expression<Func<Bank, bool>> expression)
        {
            return await _context.Banks.FirstOrDefaultAsync(expression);
        }
        public IQueryable<Bank> GetAllBanks()
        {
            return _context.Banks;
        }

        public async Task<Bank> GetBankByPaystackIdAsync(int paystackId)
        {
            return await _context.Banks.FirstOrDefaultAsync(b => b.PaystackBankId == paystackId);
        }

        public async Task<Bank> GetBankByIdAsync(string id)
        {
            return await _context.Banks.FirstOrDefaultAsync(b => b.Id == id);
        }
    }
}
