using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace UniversityApp.Api.UserPanel
{
    [ApiExplorerSettings(GroupName = "user_api_v1")]
    [Route("api/[controller]")]
    [ApiController]
    public class GroupsController : ControllerBase
    {
        [HttpGet("test")]
        public IActionResult Get()
        {
            return Ok();
        }
    }
}
