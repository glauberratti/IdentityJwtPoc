using IdentityJwtPoc.Domain.Repository;
using Microsoft.EntityFrameworkCore;

namespace IdentityJwtPoc.Infra.Data.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly AppDataContext _dbContext;

        public Repository(AppDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async ValueTask<TEntity?> GetByIdAsync(Guid id)
        {
            TEntity? entity = await _dbContext.Set<TEntity>().FindAsync(id);
            if (entity != null)
                _dbContext.Entry(entity).State = EntityState.Detached;

            return entity;
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbContext.Set<TEntity>().AsNoTracking().ToListAsync();
        }

        public async Task AddAsync(TEntity entity)
        {
            await _dbContext.Set<TEntity>().AddAsync(entity);
        }

        public async Task UpdateAsync(TEntity entity)
        {
            await Task.Run(() => { _dbContext.Entry(entity).State = EntityState.Modified; });
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _dbContext.Set<TEntity>().FindAsync(id);
            _dbContext.Set<TEntity>().Remove(entity!);
        }
    }
}
