using IdentityJwtPoc.Application.DTOs;
using IdentityJwtPoc.Application.DTOs.Responses;

namespace IdentityJwtPoc.Application.Services.Interfaces
{
    public interface IIdentityService
    {
        Task<SignUpResponse> SignUp(SignUp signUp);
        Task<LoginResponse> Login(Login login);
        Task<LoginResponse> RefreshToken(string email);
        Task Logout();
        Task<bool> CreateRole(string role);
        Task<bool> AddRoleToUser(string email, string role);
        Task<bool> RemoveRoleToUser(string email, string role);
    }
}
