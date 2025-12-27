using Microsoft.AspNetCore.Mvc;
using WebServerProject.CSR.Services.Auth;
using WebServerProject.Models.Request.Auth;
using WebServerProject.Models.Response;

namespace WebServerProject.CSR.Contollers
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
            if (string.IsNullOrWhiteSpace(request.Username) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return new RegisterResponse
                {
                    success = false,
                    message = "사용자 이름, 이메일, 비밀번호는 필수 입력 항목입니다."
                };
            }

            try
            {
                RegisterResult registerResult = await _authService.RegisterAsync(request.Username, request.Email, request.Password);
                return new RegisterResponse
                {
                    success = registerResult.success,
                    message = registerResult.message,
                    userId = registerResult.userId
                };
            }
            catch(InvalidOperationException ex)
            {
                return new RegisterResponse
                {
                    success = false,
                    message = "회원가입 중 오류가 발생했습니다."
                };
            }
            catch (Exception ex)
            {
                return new RegisterResponse
                {
                    success = false,
                    message = "회원가입 중 오류가 발생했습니다."
                };
            }
        }

        [HttpPost("login")]
        public async Task<LoginResponse> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Username) ||
                string.IsNullOrEmpty(request.Password))
            {
                return new LoginResponse
                {
                    success = false,
                    message = "사용자 이름, 비밀번호, 기기 ID는 필수 입력 항목입니다."
                };
            }

            try
            {
                LoginResult loginResult = await _authService.LoginAsync(request.Username, request.Password);

                return new LoginResponse
                {
                    success = loginResult.success,
                    message = loginResult.message,
                    token = loginResult.token.Token,
                    userId = loginResult.token.UserId,
                    sername = loginResult.token.Username,
                    expiresAt = loginResult.token.ExpiresAt
                };
            }
            catch (InvalidOperationException ex)
            {
                return new LoginResponse
                {
                    success = false,
                    message = $"유효하지 않은 작업입니다: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                return new LoginResponse
                {
                    success = false,
                    message = $"등록 중 오류가 발생했습니다: {ex.Message}"
                };
            }
        }

        [HttpPost("logout")]
        public async Task<LogoutResponse> Logout([FromBody] LogoutRequest request)
        {
            if (string.IsNullOrEmpty(request.Token))
            {
                return new LogoutResponse
                {
                    success = false,
                    message = "토큰이 필요합니다."
                };
            }

            try
            {
                bool success = await _authService.LogoutAsync(request.Token);

                return new LogoutResponse
                {
                    success = success,
                    message = success ? "로그아웃 되었습니다." : "로그아웃 처리 중 오류가 발생했습니다."
                };
            }
            catch (InvalidOperationException ex)
            {
                return new LogoutResponse
                {
                    success = false,
                    message = $"유효하지 않은 작업입니다: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                return new LogoutResponse
                {
                    success = false,
                    message = $"등록 중 오류가 발생했습니다: {ex.Message}"
                };
            }
        }

        [HttpPost("change-password")]
        public async Task<ChangePasswordResponse> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (string.IsNullOrEmpty(request.CurrentPassword) ||
                string.IsNullOrEmpty(request.NewPassword))
            {
                return new ChangePasswordResponse
                {
                    success = false,
                    message = "현재 비밀번호와 새 비밀번호는 필수 입력 항목입니다."
                };
            }

            try
            {
                bool success = await _authService.ChangePasswordAsync(request.UserId, request.CurrentPassword, request.NewPassword);

                return new ChangePasswordResponse
                {
                    success = success,
                    message = success ? "비밀번호가 변경되었습니다." : "비밀번호 변경에 실패했습니다."
                };
            }
            catch (InvalidOperationException ex)
            {
                return new ChangePasswordResponse
                {
                    success = false,
                    message = $"유효하지 않은 작업입니다: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                return new ChangePasswordResponse
                {
                    success = false,
                    message = $"등록 중 오류가 발생했습니다: {ex.Message}"
                };
            }
        }
    }
}