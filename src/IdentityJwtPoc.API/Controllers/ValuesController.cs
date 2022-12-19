using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityJwtPoc.API.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        [HttpGet]
        [Route("sem-restricao")]
        public async Task<IActionResult> Get()
        {
            return Ok();
        }

        [HttpGet]
        [Route("com-authorize")]
        [Authorize]
        public async Task<IActionResult> Get2()
        {
            return Ok();
        }

        [HttpGet]
        [Route("role-admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Get3()
        {
            return Ok();
        }

        [HttpGet]
        [Route("role-rh")]
        [Authorize(Roles = "RH")]
        public async Task<ActionResult> Get4()
        {
            return Ok();
        }
    }
}
