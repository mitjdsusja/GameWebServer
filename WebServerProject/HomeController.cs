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

        [HttpGet("check-tutorial")]
        public IActionResult CheckTutorial([FromQuery] string userId)
        {
            var user = _db.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null) return NotFound("User not found");

            return Ok(new { user.UserId, user.TutorialCompleted });
        }
    }
}