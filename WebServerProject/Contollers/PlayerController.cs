using Microsoft.AspNetCore.Mvc;
using WebServerProject.Data;
using WebServerProject.Services;

namespace WebServerProject.Contollers
{
    [ApiController]
    [Route("api/[controller]")]

    public class PlayerController : ControllerBase
    {
        private readonly PlayerService _service;

        public PlayerController(PlayerService service)
        {
            _service = service;
        }

        [HttpGet("info")]
        public IActionResult GetPlayerInfo([FromQuery] string userId)
        {
            try
            {
                var user = _service.GetPlayerInfo(userId);
                
                return Ok(new
                {
                    user.userId,
                    user.nickname,
                    user.level,
                    user.gold,
                    user.diamonds,
                    user.profileId,
                });
            }
            catch(Exception ex)
            {
                if (ex.Message == "User not found")
                    return NotFound(new { error = ex.Message });
                else
                    return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
