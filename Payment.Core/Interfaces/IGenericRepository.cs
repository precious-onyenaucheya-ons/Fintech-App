using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Core.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        /// <summary>
        /// Adds an object of type T to the database context
        /// </summary>
        /// <param name="item">Object of type T to be added to database context</param>
        /// <returns></returns>
        public Task AddAsync(T item);

        /// <summary>
        /// Updates the value of an object of type T in the database context
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public void Update(T item);

        /// <summary>
        /// Deletes an object of type T from the database context
        /// </summary>
        /// <param name="id">ID of the object to be deleted from the database context</param>
        /// <returns></returns>
        public void Delete(T entity);

        /// <summary>
        /// Deletes a range of objects of type T from the database context
        /// </summary>
        /// <param name="ids">List of IDs of entities to be deleted from the database context</param>
        /// <returns></returns>
        public void DeleteRange(IEnumerable<T> entities);

        /// <summary>
        /// Get all the beneficiary in database
        /// </summary>
        /// <param></param>
        /// <returns></returns>
    }
}
