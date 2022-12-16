using Microsoft.IdentityModel.Tokens;

namespace IdentityJwtPoc.Infra.Identity.Configurations
{
    public class JwtOptions
    {
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public SigningCredentials? SigningCredentials { get; set; }
        public int AccessTokenExpiration { get; set; }
        public int RefreshTokenExpiration { get; set; }
    }
}
