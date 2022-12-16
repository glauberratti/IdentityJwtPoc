using IdentityJwtPoc.Domain.Entities;
using IdentityJwtPoc.Infra.Data.Data.Configuration;
using Microsoft.EntityFrameworkCore;

namespace IdentityJwtPoc.Infra.Data
{
    public class AppDataContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public AppDataContext(DbContextOptions<AppDataContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
        }
    }
}
