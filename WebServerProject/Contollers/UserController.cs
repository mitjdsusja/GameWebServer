using Microsoft.AspNetCore.Mvc;
using WebServerProject.Models.DTOs;
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
        public async Task<UserInfoResponse> GetUserInfo([FromBody] UserInfoRequest req)
        {
            var userTuple = await _userService.GetUserInfoAsync(req.userId);
            
            if(userTuple == null)
            {
                return new UserInfoResponse
                {
                    success = false,
                    message = "유저 정보 요청 실패"
                };
            }

            var (user, stats, profiles, resources) = userTuple.Value;

            return new UserInfoResponse
            {
                success = true,
                user = user,
                stats = stats,
                profiles = profiles,
                resources = resources
            };
        }
    }
}
