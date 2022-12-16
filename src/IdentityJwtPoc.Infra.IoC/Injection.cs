using IdentityJwtPoc.Application.Services;
using IdentityJwtPoc.Application.Services.Interfaces;
using IdentityJwtPoc.Domain.Repository;
using IdentityJwtPoc.Domain.Repository.Repositories;
using IdentityJwtPoc.Infra.Data.Repository;
using IdentityJwtPoc.Infra.Data.Repository.Repositories;
using IdentityJwtPoc.Infra.Identity.Data;
using IdentityJwtPoc.Infra.Identity.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityJwtPoc.Infra.IoC
{
    public class Injection
    {
        public static void RegisterDI(IServiceCollection services)
        {
            services.AddDefaultIdentity<IdentityUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<IdentityDataContext>()
                .AddDefaultTokenProviders();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICookieService, CookieService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }
    }
}
