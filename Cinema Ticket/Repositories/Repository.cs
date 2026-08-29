using Cinema_Ticket.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace Cinema_Ticket.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDBcContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(ApplicationDBcContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<EntityEntry<T>> InsertAsync(T entity)
        {
            return await _dbSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        private IQueryable<T> Query(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<T, object>>[]? includes = null,
            bool IsTraked = true)
        {
            var entities = _dbSet.AsQueryable();

            if (filter != null)
            {
                entities = entities.Where(filter);
            }

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    entities = entities.Include(include);
                }
            }

            if (!IsTraked)
            {
                entities = entities.AsNoTracking();
            }

            return entities;
        }

        public async Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<T, object>>[]? includes = null,
            bool IsTraked = true)
        {
            var entities = Query(
                filter,
                includes,
                IsTraked);

            return await entities.ToListAsync();
        }

        public async Task<T> GetOneAsync(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<T, object>>[]? includes = null,
            bool IsTraked = true)
        {
            var entities = Query(
                filter,
                includes,
                IsTraked);

            return await entities.FirstOrDefaultAsync();
        }

        public async Task<int> CommitAsync()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
    }
}