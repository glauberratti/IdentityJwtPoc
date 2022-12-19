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
        public IActionResult Get()
        {
            return Ok();
        }

        [HttpGet]
        [Route("com-authorize")]
        [Authorize]
        public IActionResult Get2()
        {
            return Ok();
        }

        [HttpGet]
        [Route("role-admin")]
        [Authorize(Roles = "Admin")]
        public IActionResult Get3()
        {
            return Ok();
        }

        [HttpGet]
        [Route("role-rh")]
        [Authorize(Roles = "RH")]
        public IActionResult Get4()
        {
            return Ok();
        }
    }
}
