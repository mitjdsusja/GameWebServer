using Microsoft.AspNetCore.Mvc;
using WebServerProject.Data;

namespace WebServerProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly GameDbContext _db;
        public HomeController(GameDbContext db) => _db = db;

        [HttpGet("init-home")]
        public IActionResult InitHome([FromQuery] string userId)
        {
            var user = _db.users.FirstOrDefault(u => u.userId == userId);
            if (user == null) return NotFound("User not found");

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

        [HttpGet("check-tutorial")]
        public IActionResult CheckTutorial([FromQuery] string userId)
        {
            var user = _db.users.FirstOrDefault(u => u.userId == userId);
            if (user == null) return NotFound("User not found");

            return Ok(new { user.userId, user.tutorialCompleted });
        }
    }
}