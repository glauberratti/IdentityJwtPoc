using IdentityJwtPoc.Application.DTOs;
using IdentityJwtPoc.Application.DTOs.Responses;
using IdentityJwtPoc.Application.Services.Interfaces;
using IdentityJwtPoc.Domain;
using IdentityJwtPoc.Domain.Entities;
using IdentityJwtPoc.Domain.Repository;
using IdentityJwtPoc.Infra.Data.CrossCutting.Cryptography;
using IdentityJwtPoc.Infra.Identity.Configurations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace IdentityJwtPoc.Infra.Identity.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtOptions _jwtOptions;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _uow;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;
        private readonly ICookieService _cookieService;

        public IdentityService(SignInManager<IdentityUser> signInManager,
                               UserManager<IdentityUser> userManager,
                               IOptions<JwtOptions> jwtOptions,
                               RoleManager<IdentityRole> roleManager,
                               IUnitOfWork unitOfWork,
                               IHttpContextAccessor httpContextAccessor,
                               IUserService userService,
                               ICookieService cookieService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtOptions = jwtOptions.Value;
            _roleManager = roleManager;
            _uow = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
            _cookieService = cookieService;
        }

        public async Task<SignUpResponse> SignUp(SignUp signUp)
        {
            var id = Guid.NewGuid();
            var identityUser = new IdentityUser
            {
                Id = id.ToString(),
                UserName = signUp.Email,
                Email = signUp.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(identityUser, signUp.Password);
            var signUpResponse = new SignUpResponse(result.Succeeded);

            if (result.Succeeded)
            {
                await _userManager.SetLockoutEnabledAsync(identityUser, false);
                var user = new User()
                {
                    Id = id,
                    Name = signUp.Name,
                    Email = signUp.Email,
                    Phone = signUp.Phone,
                    Active = true,
                    LastChange = DateTime.Now,
                };
                _uow.BeginTransaction();
                await _userService.Add(user);
                var commitOk = _uow.Commit();

                if (!commitOk)
                {
                    signUpResponse.Success = false;
                    signUpResponse.AddError("Erro ao criar usuário, tente novamente");
                    _uow.Rollback();
                }
            }
            else
            {
                signUpResponse.AddErrors(result.Errors.Select(r => r.Description));
            }

            return signUpResponse;
        }

        public async Task<LoginResponse> Login(Login login)
        {
            var token = string.Empty;
            var refreshToken = string.Empty;
            var result = await _signInManager.PasswordSignInAsync(login.Email, login.Password, false, true);

            if (result.Succeeded)
            {
                token = await CreateToken(login.Email);
                refreshToken = await CreateRefreshToken(login.Email);
            }

            var loginResponse = new LoginResponse(token, refreshToken);
            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                    loginResponse.AddError("Essa conta está bloqueada");
                else if (result.IsNotAllowed)
                    loginResponse.AddError("Essa conta não tem permissão para fazer login");
                else if (result.RequiresTwoFactor)
                    loginResponse.AddError("É necessário confirmar o login no seu segundo fator de autenticação");
                else
                    loginResponse.AddError("Usuário ou senha estão incorretos");
            }

            return loginResponse;
        }

        public async Task<LoginResponse> RefreshToken()
        {
            var cookieRefreshTokenFingerPrint = _cookieService.GetCookieValue(Claims.RTFB) ?? "";

            if (cookieRefreshTokenFingerPrint == "")
                return ResponseRefreshTokenError();

            var claimRefreshTokenFingerPrint = _httpContextAccessor?.HttpContext?.User?.Claims?
                .FirstOrDefault(c => c.Type == Claims.FB)?.Value ?? "";

            if (claimRefreshTokenFingerPrint == "")
                return ResponseRefreshTokenError();

            var decryptedClaimRefreshTokenFingerPrint = Cryptography.DecryptString(claimRefreshTokenFingerPrint);

            if (decryptedClaimRefreshTokenFingerPrint != cookieRefreshTokenFingerPrint)
                return ResponseRefreshTokenError();

            var userId = _httpContextAccessor?.HttpContext?.User?.Claims?
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "";

            if (userId == "")
                return ResponseRefreshTokenError();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return ResponseRefreshTokenError();

            var token = await CreateToken(user.Email);
            var refreshToken = await CreateRefreshToken(user.Email);
            
            return new LoginResponse(token, refreshToken);
        }

        private LoginResponse ResponseRefreshTokenError()
        {
            Logout();
            var response = new LoginResponse("", "");
            response.AddError("Sem permissão para realizar essa operação");
            return response;
        }

        public void Logout()
        {
            _cookieService.DeleteCookie(Claims.FB);
            _cookieService.DeleteCookie(Claims.RTFB);
        }

        private async Task<string> CreateToken(string email)
        {
            var fingerPrint = Guid.NewGuid().ToString();
            var encryptedFingerPrint = Cryptography.EncryptString(fingerPrint);
            var identityUser = await _userManager.FindByEmailAsync(email);
            var user = await _userService.GetById(new Guid(identityUser.Id));
            var claims = await GetClaims(identityUser);
            var expirationTime = DateTime.Now.AddSeconds(_jwtOptions.AccessTokenExpiration);

            claims.Add(new Claim(Claims.LAST_CHANGE, user!.LastChange.ToString()));
            claims.Add(new Claim(Claims.FB, encryptedFingerPrint));

            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                notBefore: DateTime.Now,
                expires: expirationTime,
                signingCredentials: _jwtOptions.SigningCredentials
                );

            var token = new JwtSecurityTokenHandler().WriteToken(jwt);

            _cookieService.CreateCookie(Claims.FB, fingerPrint, _jwtOptions.AccessTokenExpiration);

            return token;
        }

        private async Task<string> CreateRefreshToken(string email)
        {
            var refreshTokenFingerPrint = Guid.NewGuid().ToString();
            var encryptedFingerPrint = Cryptography.EncryptString(refreshTokenFingerPrint);
            var identityUser = await _userManager.FindByEmailAsync(email);
            var expirationTime = DateTime.Now.AddSeconds(_jwtOptions.RefreshTokenExpiration);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, identityUser.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Nbf, DateTime.Now.ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToString()),
                new Claim(Claims.FB, encryptedFingerPrint),
            };

            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                notBefore: DateTime.Now,
                expires: expirationTime,
                signingCredentials: _jwtOptions.SigningCredentials
                );

            var refreshToken = new JwtSecurityTokenHandler().WriteToken(jwt);

            _cookieService.CreateCookie(Claims.RTFB, refreshTokenFingerPrint, _jwtOptions.RefreshTokenExpiration);

            return refreshToken;
        }

        private async Task<IList<Claim>> GetClaims(IdentityUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Nbf, DateTime.Now.ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToString())
            };

            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
                claims.Add(new Claim("role", role));

            return claims;
        }

        public async Task<bool> CreateRole(string role)
        {
            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole(role));

            return true;
        }

        public async Task<IEnumerable<string>> GetUserRoles(string email)
        {
            var identityUser = await _userManager.FindByEmailAsync(email);
            var user = await _userService.GetById(new Guid(identityUser.Id));

            if (identityUser is null || user is null)
                return new List<string>();

            return await _userManager.GetRolesAsync(identityUser);
        }

        public async Task<bool> AddRoleToUser(string email, string role)
        {
            var identityUser = await _userManager.FindByEmailAsync(email);
            var user = await _userService.GetById(new Guid(identityUser.Id));

            if (identityUser is null || user is null)
                return true;

            if (!await _roleManager.RoleExistsAsync(role))
                return true;

            var result = await _userManager.AddToRoleAsync(identityUser, role);
            if (result.Succeeded)
            {
                user.LastChange = DateTime.Now;
                _uow.BeginTransaction();
                await _userService.Update(user);
                var ok = _uow.Commit();

                if (!ok)
                {
                    throw new Exception("Ocorreu algum erro no BD");
                }
            }

            return true;
        }

        public async Task<bool> RemoveRoleToUser(string email, string role)
        {
            var identityUser = await _userManager.FindByEmailAsync(email);
            var user = await _userService.GetById(new Guid(identityUser.Id));

            if (identityUser is null || user is null)
                return true;

            if (!await _roleManager.RoleExistsAsync(role))
                return true;

            if (!await _roleManager.RoleExistsAsync(role))
                return true;

            var result = await _userManager.RemoveFromRoleAsync(identityUser, role);
            if (result.Succeeded)
            {
                user.LastChange = DateTime.Now;
                _uow.BeginTransaction();
                await _userService.Update(user);
                var ok = _uow.Commit();

                if (!ok)
                {
                    throw new Exception("Ocorreu algum erro no BD");
                }
            }

            return true;
        }
    }
}
