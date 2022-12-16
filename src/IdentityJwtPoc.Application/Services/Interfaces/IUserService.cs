using IdentityJwtPoc.Domain.Entities;

namespace IdentityJwtPoc.Application.Services.Interfaces
{
    public interface IUserService
    {
        ValueTask<User?> GetById(Guid id);
        Task<IEnumerable<User>> GetAll();
        Task Add(User user);
        Task Update(User user);
        Task Delete(Guid id);
    }
}
