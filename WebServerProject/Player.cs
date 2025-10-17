using Microsoft.AspNetCore.Mvc;
using WebServerProject.Data;

namespace WebServerProject
{
    [ApiController]
    [Route("api/[controller]")]

    public class PlayerController : ControllerBase
    {
        private readonly GameDbContext _db;
        public PlayerController(GameDbContext db) => _db = db;

        [HttpGet("info")]
        public IActionResult GetPlayerInfo([FromQuery] string userId)
        {
            var user = _db.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null) return NotFound("User not found");

            return Ok(new
            {
                user.UserId,
                user.Nickname,
                user.Level,
                user.Gold,
                user.Diamonds,
                user.ProfileId,
            });
        }
    }
}
