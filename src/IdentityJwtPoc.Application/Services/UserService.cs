using IdentityJwtPoc.Application.Services.Interfaces;
using IdentityJwtPoc.Domain.Entities;
using IdentityJwtPoc.Domain.Repository;
using IdentityJwtPoc.Domain.Repository.Repositories;

namespace IdentityJwtPoc.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _uow;
        private readonly IUserRepository _userRepository;

        public UserService(IUnitOfWork uow, IUserRepository userRepository)
        {
            _uow = uow;
            _userRepository = userRepository;
        }

        public async ValueTask<User?> GetById(Guid id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task Add(User user, bool userTransaction = true)
        {
            if (user.Id == Guid.Empty)
                user.Id = Guid.NewGuid();

            if (userTransaction)
             _uow.BeginTransaction();
            
            await _userRepository.AddAsync(user);
            
            if (userTransaction)
            {
                var ok = _uow.Commit();
                if (!ok)
                {
                    throw new Exception("Ocorreu algum erro no BD");
                }
            }
        }

        public async Task Update(User user, bool userTransaction = true)
        {
            user.LastChange = DateTime.Now;

            if (userTransaction)
                _uow.BeginTransaction();

            await _userRepository.UpdateAsync(user);

            if (userTransaction)
            {
                var ok = _uow.Commit();
                if (!ok)
                {
                    throw new Exception("Ocorreu algum erro no BD");
                }
            }
        }

        public async Task Delete(Guid id, bool userTransaction = true)
        {
            if (userTransaction)
                _uow.BeginTransaction();

            await _userRepository.DeleteAsync(id);

            if (userTransaction)
            {
                var ok = _uow.Commit();
                if (!ok)
                {
                    throw new Exception("Ocorreu algum erro no BD");
                }
            }
        }
    }
}
