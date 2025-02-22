using System.Linq.Expressions;

namespace ReservationApp.Data.IRepository
{
    public interface IRepository<T> where T : class
    {
        public void Add(T entity);
        public void Remove(T entity);
        public void RemoveRange(IEnumerable<T> entity);

        public void Update(T entity);

        public T? Get(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false);
        public  IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null, bool tracked = false);
        
    }
}
