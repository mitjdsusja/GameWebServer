using Microsoft.AspNetCore.Mvc;
using WebServerProject.Data;
using WebServerProject.Services;

namespace WebServerProject.Contollers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly HomeService _service;

        public HomeController(HomeService service)
        {
            _service = service;
        }

        [HttpGet("init")]
        public async Task<IActionResult> InitHome([FromQuery] string userId)
        {
            return Ok();
        }
    }
}