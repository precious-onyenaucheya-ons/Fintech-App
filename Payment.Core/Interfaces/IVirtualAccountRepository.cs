using Payment.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Core.Interfaces
{
    public interface IVirtualAccountRepository : IGenericRepository<VirtualAccount>
    {
        Task<VirtualAccount> GetVirtualAccountByAccountNumber(string accountNumber);
    }
}
