namespace NotificationServices.Infrastructure.Repository
{
    public interface IGenericRepository<T>
    {
        Task DeleteAsync(string id);
        void DeleteRangeAsync(IEnumerable<T> entities);
        Task InsertAsync(T entity);
        Task<T> UpdateAsyn(T entity, object key);
    }
}