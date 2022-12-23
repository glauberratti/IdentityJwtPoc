using IdentityJwtPoc.Domain.Entities;

namespace IdentityJwtPoc.Application.Services.Interfaces
{
    public interface IUserService
    {
        ValueTask<User?> GetById(Guid id);
        Task<IEnumerable<User>> GetAll();
        Task Add(User user, bool userTransaction = true);
        Task Update(User user, bool userTransaction = true);
        Task Delete(Guid id, bool userTransaction = true);
    }
}
