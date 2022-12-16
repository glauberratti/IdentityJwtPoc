using System.Text.Json.Serialization;

namespace IdentityJwtPoc.API.Configuration
{
    public static class ApiConfiguration
    {
        public static IServiceCollection AddApiConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

            services.AddCors(options =>
            {
                options.AddPolicy(
                "CorsPolicy",
                  p => p.WithOrigins(configuration.GetSection("AllowedHosts").Value!)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials()
                );
            });

            return services;
        }
    }
}
