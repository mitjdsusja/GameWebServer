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
            try
            {
                var homeData = await _service.InitHomeAsync(userId);
                return Ok(homeData);
            }
            catch (Exception ex)
            {
                if (ex.Message == "User not found")
                    return NotFound(new { error = ex.Message });
                else
                    return StatusCode(500, new { error = ex.Message });
            }
        }
        
        [HttpGet("check-tutorial")]
        public async Task<IActionResult> CheckTutorial([FromQuery] string userId)
        {
            return Ok(new { });
        }
    }
}