using Microsoft.AspNetCore.Mvc;
using WebServerProject.Models.DTOs.Request;
using WebServerProject.Models.DTOs.Response;
using WebServerProject.Services;

namespace WebServerProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("info")]
        public async Task<IActionResult> GetUserInfo([FromBody] UserInfoRequest req)
        {
            var user = await _userService.GetUserInfoAsync(req.userId);

            return Ok(new UserInfoResponse
            {
                User = user
            });
        }
    }
}
