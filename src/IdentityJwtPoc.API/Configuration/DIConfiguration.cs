using IdentityJwtPoc.Infra.IoC;

namespace IdentityJwtPoc.API.Configuration
{
    public static class DIConfiguration
    {
        public static void AddDIConfiguration(this IServiceCollection services)
        {
            Injection.RegisterDI(services);
        }
    }
}
