using IdentityJwtPoc.Application.DTOs;
using IdentityJwtPoc.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityJwtPoc.API.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IIdentityService _identityService;

        public AuthController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        [HttpPost]
        [Route("signup")]
        public async Task<ActionResult> SignUp(SignUp signUp)
        {
            try
            {
                var result = await _identityService.SignUp(signUp);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> Login(Login login)
        {
            try
            {
                var result = await _identityService.Login(login);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("refresh-token")]
        public async Task<ActionResult> RefreshToken()
        {
            try
            {
                var result = await _identityService.RefreshToken();

                if (result.Errors.Count > 0)
                    return Unauthorized();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("logout")]
        public ActionResult Logout()
        {
            try
            {
                _identityService.Logout();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("get-role")]
        public async Task<ActionResult> getRoles(string email)
        {
            try
            {
                return Ok(await _identityService.GetUserRoles(email));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("create-role")]
        public async Task<ActionResult> CreateRole(string role)
        {
            try
            {
                await _identityService.CreateRole(role);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("add-role")]
        public async Task<ActionResult> AddRoleToUser(string email, string role)
        {
            try
            {
                await _identityService.AddRoleToUser(email, role);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("remove-role")]
        public async Task<ActionResult> RemoveRoleToUser(string email, string role)
        {
            try
            {
                await _identityService.RemoveRoleToUser(email, role);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
