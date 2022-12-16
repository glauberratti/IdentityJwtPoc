using IdentityJwtPoc.Domain.Repository;
using IdentityJwtPoc.Infra.Data.Data;

namespace IdentityJwtPoc.Infra.Data.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDataContext _dbContext;

        public UnitOfWork(AppDataContext entityContext)
        {
            _dbContext = entityContext;
        }

        public void BeginTransaction()
        {
            _dbContext.Database.BeginTransaction();
        }

        public bool Commit()
        {
            try
            {
                var rowsAffected = _dbContext.SaveChanges();

                if (_dbContext.Database.CurrentTransaction == null)
                    _dbContext.Database.CommitTransaction();
                else
                    _dbContext.Database.CurrentTransaction!.Commit();

                return true;
            }
            catch (Exception)
            {
                Rollback();
                return false;
            }
            finally
            {
                Dispose();
            }
        }

        public void Rollback()
        {
            if (_dbContext.Database.CurrentTransaction == null)
                _dbContext.Database.RollbackTransaction();
            else
                _dbContext.Database.CurrentTransaction.Rollback();

            Dispose();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
