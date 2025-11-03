using Microsoft.AspNetCore.Mvc;
using WebServerProject.Models.DTOs.Request;
using WebServerProject.Models.DTOs.Response;
using WebServerProject.Services;

namespace WebServerProject.Contollers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<RegisterResponse> Register([FromBody] RegisterRequest request)
        {
            if (string.IsNullOrEmpty(request.Username) ||
                string.IsNullOrEmpty(request.Email) ||
                string.IsNullOrEmpty(request.Password))
            {
                return new RegisterResponse
                {
                    Success = false,
                    Message = "사용자 이름, 이메일, 비밀번호는 필수 입력 항목입니다."
                };
            }

            var (success, message, userId) = await _authService.RegisterAsync(
                request.Username, request.Email, request.Password);

            return new RegisterResponse
            {
                Success = success,
                Message = message,
                UserId = userId
            };
        }

        [HttpPost("login")]
        public async Task<LoginResponse> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Username) ||
                string.IsNullOrEmpty(request.Password))
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "사용자 이름, 비밀번호, 기기 ID는 필수 입력 항목입니다."
                };
            }

            var (success, message, token) = await _authService.LoginAsync(
                request.Username, request.Password);

            return new LoginResponse
            {
                Success = success,
                Message = message,
                Token = token?.Token,
                UserId = token?.UserId,
                Username = token?.Username,
                ExpiresAt = token?.ExpiresAt
            };
        }

        [HttpPost("logout")]
        public async Task<LogoutResponse> Logout([FromBody] LogoutRequest request)
        {
            if (string.IsNullOrEmpty(request.Token))
            {
                return new LogoutResponse
                {
                    Success = false,
                    Message = "토큰이 필요합니다."
                };
            }

            bool success = await _authService.LogoutAsync(request.Token);

            return new LogoutResponse
            {
                Success = success,
                Message = success ? "로그아웃 되었습니다." : "로그아웃 처리 중 오류가 발생했습니다."
            };
        }

        [HttpPost("change-password")]
        public async Task<ChangePasswordResponse> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (string.IsNullOrEmpty(request.CurrentPassword) ||
                string.IsNullOrEmpty(request.NewPassword))
            {
                return new ChangePasswordResponse
                {
                    Success = false,
                    Message = "현재 비밀번호와 새 비밀번호는 필수 입력 항목입니다."
                };
            }

            bool success = await _authService.ChangePasswordAsync(
                request.UserId, request.CurrentPassword, request.NewPassword);

            return new ChangePasswordResponse
            {
                Success = success,
                Message = success ? "비밀번호가 변경되었습니다." : "비밀번호 변경에 실패했습니다."
            };
        }
    }
}