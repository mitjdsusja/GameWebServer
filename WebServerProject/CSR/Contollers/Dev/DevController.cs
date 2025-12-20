using Microsoft.AspNetCore.Mvc;
using WebServerProject.CSR.Services.Auth;

namespace WebServerProject.CSR.Contollers.Dev
{
    [ApiController]
    [Route("api/[controller]")]
    public class DevController : ControllerBase
    {
        private readonly ILogger<DevController> _logger;
        private readonly IAuthService _authService;

        public DevController(
            ILogger<DevController> logger,
            IAuthService authService)
        {
            _logger = logger;
            _authService = authService;
        }

        [HttpPost("create-dummy-user")]
        public async Task<IActionResult> CreateDummyUser([FromBody] CreateDummyUserRequest reqeust)
        {
            try
            {
                for(int i=1; i <= reqeust.createCount; i++)
                {
                    string username = "testuser" + i.ToString();
                    string email = "testuser" + i.ToString() + "@example.com";
                    string password = "test1234";

                    await _authService.RegisterAsync(username, email, password);
                }

                return Ok($"{reqeust.createCount}개의 더미 유저가 성공적으로 생성되었습니다.");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "더미 유저 생성 중 오류가 발생했습니다.");
                return StatusCode(500, "더미 유저 생성 중 오류가 발생했습니다.");
            }
        }
    }
}
