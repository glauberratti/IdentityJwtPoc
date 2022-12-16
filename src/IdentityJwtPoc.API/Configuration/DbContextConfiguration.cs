using IdentityJwtPoc.Infra.Data;
using IdentityJwtPoc.Infra.Identity.Data;
using Microsoft.EntityFrameworkCore;

namespace IdentityJwtPoc.API.Configuration
{
    public static class DbContextConfiguration
    {
        public static IServiceCollection AddDbContextConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDataContext>(opt => opt.UseSqlite(configuration.GetConnectionString("SQLite")));
            services.AddDbContext<IdentityDataContext>(opt => opt.UseSqlite(configuration.GetConnectionString("SQLite")));

            return services;
        }
    }
}
