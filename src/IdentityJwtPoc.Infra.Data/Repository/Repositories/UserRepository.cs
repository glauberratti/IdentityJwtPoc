using IdentityJwtPoc.Domain.Entities;
using IdentityJwtPoc.Domain.Repository.Repositories;
using IdentityJwtPoc.Infra.Data.Data;

namespace IdentityJwtPoc.Infra.Data.Repository.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(AppDataContext _dbContext) : base(_dbContext)
        {

        }
    }
}
