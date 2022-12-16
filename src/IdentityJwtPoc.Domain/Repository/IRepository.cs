namespace IdentityJwtPoc.Domain.Repository
{
    public interface IRepository<TEntity> where TEntity : class
    {
        ValueTask<TEntity?> GetByIdAsync(Guid id);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(Guid id);
    }
}
