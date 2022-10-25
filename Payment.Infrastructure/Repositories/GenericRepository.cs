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
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly PaymentDbContext _dbContext;
        private readonly DbSet<T> _entityDbSet;

        public GenericRepository(PaymentDbContext dbContext)
        {
            _dbContext = dbContext;
            _entityDbSet = dbContext.Set<T>();
        }

        public async Task AddAsync(T item)
        {
          
            await _entityDbSet.AddAsync(item);
        }

        public void Delete(T entity)
        {
            _entityDbSet.Remove(entity);
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            _entityDbSet.RemoveRange(entities);
        }

        public void Update(T item)
        {
            _dbContext.Attach(item);
            _dbContext.Entry(item).State = EntityState.Modified;
        }
    }
}
