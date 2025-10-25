using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WebServerProject.Services;

namespace WebServerProject.Contollers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _service;

        public AuthController(AuthService service)
        {
            _service = service;
        }
        // ---------------- Guest 로그인 ----------------
        public record GuestLoginRequest();

        [HttpPost("guest-login")]
        public IActionResult GuestLogin([FromBody] GuestLoginRequest req)
        {
            try
            {
                var user = _service.GuestLogin();
                return Ok(new { user.userId });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ GuestLogin Error: {ex.Message}");
                return StatusCode(500, new { error = ex.Message });
            }
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

        [HttpGet("check-uid")]
        public IActionResult CheckUID([FromQuery] string userId)
        {
            try
            {
                bool exists = _service.CheckUID(userId);

                if (exists)
                    return Ok(new { userId });
                else
                    return NotFound(new { error = "UID not found" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ CheckUID Error: {ex.Message}");
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
