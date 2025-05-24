using ACS_Reception.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using System.Linq.Expressions;

namespace ACS_Reception.Persistence.Repository
{
    public class EfRepository<T>(AppDbContext context) : IRepository<T> where T : Entity
    {
        protected readonly AppDbContext context = context;
        protected readonly DbSet<T> entities = context.Set<T>();

        public Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            entities.Add(entity);

            return Task.CompletedTask;
        }

        public Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            entities.Remove(entity);

            return Task.CompletedTask;
        }

        public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default)
        {
            return (await entities.FirstOrDefaultAsync(filter, cancellationToken))!;
        }

        public async Task<T> GetByIdAsync(ObjectId id, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[]? includesProperties)
        {
            IQueryable<T>? query = entities.AsQueryable();
            if (includesProperties is not null && includesProperties.Length != 0)
            {
                foreach (Expression<Func<T, object>> included in includesProperties)
                {
                    query = query.Include(included);
                }
            }

            query.Where(e => e.Id == id);

            return (await query.FirstOrDefaultAsync(cancellationToken))!;
        }

        public async Task<IReadOnlyList<T>> ListAllAsync(CancellationToken cancellationToken = default)
        {
            return await entities.ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[]? includesProperties)
        {
            IQueryable<T>? query = entities.AsQueryable();
            if (includesProperties is not null && includesProperties.Length != 0)
            {
                foreach (Expression<Func<T, object>>? included in includesProperties!)
                {
                    query = query.Include(filter);
                }
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.ToListAsync();
        }

        public Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            context.Entry(entity).State = EntityState.Modified;

            return Task.CompletedTask;
        }
    }
}
