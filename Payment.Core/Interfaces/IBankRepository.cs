using Payment.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Core.Interfaces
{
    public interface IBankRepository : IGenericRepository<Bank>
    {
        /// <summary>
        /// Retrieves a Bank object which has a matching PaystackId property
        /// </summary>
        /// <param name="paystackId">id used to get required bank object</param>
        /// <returns>The bank object if found, or null if not found</returns>
        Task<Bank> GetBankByPaystackIdAsync(int paystackId);
        /// <summary>
        /// Retrieves a Bank object which has a matching Id property
        /// </summary>
        /// <param name="id">id used to get required bank object</param>
        /// <returns>The bank object if found, or null if not found</returns>
        Task<Bank> GetBankByIdAsync(string id);
        /// <summary>
        /// Retrieves all Bank objects from the database
        /// </summary>
        /// <returns>An IQueryable of the bank objects</returns>
        IQueryable<Bank> GetAllBanks();
    }
}
