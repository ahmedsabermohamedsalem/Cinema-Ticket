using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Cinema_Ticket.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<EntityEntry<T>> InsertAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<IEnumerable<T>> GetAllAsync(
         Expression<Func<T, bool>>? filter = null,
         Expression<Func<T, object>>[]? includes = null,
         bool IsTraked = true
         );
        Task<T> GetOneAsync(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<T, object>>[]? includes = null,
            bool IsTraked = true
            );

        Task<int> CommitAsync();
    }
}

