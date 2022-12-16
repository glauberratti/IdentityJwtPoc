using IdentityJwtPoc.Application.Services.Interfaces;
using IdentityJwtPoc.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace IdentityJwtPoc.API.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(Guid id)
        {
            try
            {
                var department = await _userService.GetById(id);

                if (department == null)
                    return NotFound();

                return Ok(department);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            try
            {
                var department = await _userService.GetAll();
                return Ok(department);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Add([FromBody] User user)
        {
            try
            {
                await _userService.Add(user);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult> Update([FromBody] User user)
        {
            try
            {
                var employeRes = await _userService.GetById(user.Id);

                if (employeRes == null)
                    return NotFound();

                await _userService.Update(user);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                var department = await _userService.GetById(id);
                if (department == null)
                    return NotFound();

                await _userService.Delete(id);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
