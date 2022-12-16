namespace IdentityJwtPoc.Domain.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        public void BeginTransaction();
        bool Commit();
        void Rollback();
        new void Dispose();
    }
}
