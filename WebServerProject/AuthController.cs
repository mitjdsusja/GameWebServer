using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WebServerProject.Data;

namespace WebServerProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly GameDbContext _db;

        public AuthController(GameDbContext db)
        {
            _db = db;
        }

        // ---------------- Guest 로그인 ----------------
        public record GuestLoginRequest();

        [HttpPost("guest-login")]
        public IActionResult GuestLogin([FromBody] GuestLoginRequest req)
        {
            var user = new User
            {
                userId = Guid.NewGuid().ToString(),
                nickname = "Guest"
            };

            _db.users.Add(user);
            _db.SaveChanges();

            return Ok(new { user.userId });
        }

        // ---------------- Google 로그인 ----------------
        public record GoogleLoginRequest(string IdToken);

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest req)
        {
            if (string.IsNullOrEmpty(req.IdToken))
                return BadRequest("IdToken is required");

            string email;

            try
            {
                using var http = new HttpClient();
                var response = await http.GetStringAsync(
                    $"https://oauth2.googleapis.com/tokeninfo?id_token={req.IdToken}");
                var json = JsonSerializer.Deserialize<JsonElement>(response);

                // Web Client ID 검증
                var aud = json.GetProperty("aud").GetString();
                if (aud != "여기에_발급받은_Web_Client_ID")
                    return BadRequest("Invalid client ID");

                email = json.GetProperty("email").GetString();
            }
            catch
            {
                return BadRequest("Invalid ID Token");
            }

            // 유저 ID 생성 (실제 서비스에서는 DB 확인 후 신규 등록)
            var userId = email;

            // JWT 발급 (테스트용 GUID)
            var token = Guid.NewGuid().ToString();

            return Ok(new { userId, email, token });
        }

        public record SetNicknameRequest(string UserId, string Nickname);

        [HttpPost("set-nickname")]
        public IActionResult SetNickname([FromBody] SetNicknameRequest req)
        {
            var user = _db.users.FirstOrDefault(u => u.userId == req.UserId);
            if (user == null)
                return NotFound("User not found");

            if (string.IsNullOrEmpty(req.Nickname))
                return BadRequest("Nickname is required");

            user.nickname = req.Nickname;
            _db.SaveChanges();

            return Ok(new { user.userId, user.nickname });
        }

        [HttpGet("check-uid")]
        public IActionResult CheckUID([FromQuery] string userId)
        {
            var user = _db.users.FirstOrDefault(u => u.userId == userId);
            if (user == null)
                return NotFound("User not found");

            return Ok(new { userId });
        }
    }
}
