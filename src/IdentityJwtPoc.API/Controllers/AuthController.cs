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
        public async Task<ActionResult> RefreshToken(string email)
        {
            try
            {
                var teste = HttpContext.User.Claims;
                var result = await _identityService.RefreshToken(email);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("logout")]
        public async Task<ActionResult> Logout()
        {
            try
            {
                await _identityService.Logout();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Authorize(Policy = "Admin")]
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

        [Authorize(Policy = "Admin")]
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

        [Authorize(Policy = "Admin")]
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
