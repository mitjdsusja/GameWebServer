using Humanizer.Localisation;
using Microsoft.AspNetCore.Mvc;
using WebServerProject.CSR.Services;
using WebServerProject.Models.DTOs;
using WebServerProject.Models.Request;
using WebServerProject.Models.Response;

namespace WebServerProject.CSR.Contollers
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
            try
            {
                var user = await _userService.GetUserDetailAsync(req.userId);

                return new UserInfoResponse
                {
                    success = true,
                    message = "유저 정보를 불러왔습니다.",
                    user = user,
                };
            }
            catch(InvalidOperationException ex)
            {
                return new UserInfoResponse
                {
                    success = false,
                    message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new UserInfoResponse
                {
                    success = false,
                    message = "유저 정보 요청 중 오류가 발생했습니다. 관리자에게 문의하세요."
                };
            }        
        }
    }
}
