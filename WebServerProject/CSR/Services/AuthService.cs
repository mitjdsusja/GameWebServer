using WebServerProject.CSR.Repositories;
using WebServerProject.Models.Entities.User;
using WebServerProject.Models.Utils.Auth;

namespace WebServerProject.CSR.Services
{
    public interface IAuthService
    {
        Task<(bool Success, string Message, int? UserId)> RegisterAsync(string username, string email, string password);
        Task<(bool Success, string Message, AuthToken Token)> LoginAsync(string username, string password);
        Task<bool> LogoutAsync(string token);
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    }

    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IAuthTokenService _tokenService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IAuthTokenService tokenService,
            ILogger<AuthService> loger)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _logger = loger;
        }

        public async Task<(bool Success, string Message, int? UserId)> RegisterAsync(string username, string email, string password)
        {
            // 사용자 이름 중복 확인
            var existingUserByUsername = await _userRepository.GetUserByUsernameAsync(username);
            if (existingUserByUsername != null)
            {
                return (false, "이미 사용 중인 사용자 이름입니다.", null);
            }

            // 이메일 중복 확인
            var existingUserByEmail = await _userRepository.GetUserByEmailAsync(email);
            if (existingUserByEmail != null)
            {
                return (false, "이미 사용 중인 이메일 주소입니다.", null);
            }

            // 비밀번호 유효성 검사
            if (string.IsNullOrEmpty(password) || password.Length < 8)
            {
                return (false, "비밀번호는 최소 8자 이상이어야 합니다.", null);
            }

            // 비밀번호 해싱
            var (passwordHash, salt) = _passwordHasher.HashPassword(password);

            // 새 사용자 생성
            var newUser = new User
            {
                username = username,
                email = email,
                password_hash = passwordHash,
                salt = salt,
                status = "active",
            };

            try
            {
                // 데이터베이스에 저장
                int userId = await _userRepository.CreateAsync(newUser);

                return (true, "회원가입이 완료되었습니다.", userId);
            }
            catch (Exception ex)
            {
                return (false, "회원가입 처리 중 오류가 발생했습니다.", null);
            }
        }

        public async Task<(bool Success, string Message, AuthToken Token)> LoginAsync(string username, string password)
        {
            try
            {
                // 사용자 조회
                var user = await _userRepository.GetUserByUsernameAsync(username);

                if (user == null)
                {
                    return (false, "사용자 이름 또는 비밀번호가 올바르지 않습니다.", null);
                }

                // 계정 상태 확인
                if (user.status != "active")
                {
                    return (false, $"계정이 {user.status} 상태입니다. 관리자에게 문의하세요.", null);
                }

                // 비밀번호 검증
                bool isPasswordValid = _passwordHasher.VerifyPassword(password, user.password_hash, user.salt);

                if (!isPasswordValid)
                {
                    return (false, "사용자 이름 또는 비밀번호가 올바르지 않습니다.", null);
                }

                // 마지막 로그인 시간 업데이트
                await _userRepository.UpdateLastLoginAsync(user.id, DateTime.UtcNow);

                // 인증 토큰 생성
                var token = await _tokenService.CreateTokenAsync(user);
                // 인증 토큰 서버 저장
                //

                if (token == null)
                {
                    return (false, "인증 토큰 생성에 실패했습니다.", null);
                }

                return (true, "로그인에 성공했습니다.", token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LoginAsync 중 오류 발생");
                return (false, "로그인 처리 중 오류가 발생했습니다.", null);
            }
        }

        public async Task<bool> LogoutAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }

            try
            {
                return await _tokenService.RevokeTokenAsync(token);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            try
            {
                             // 사용자 조회
                var user = await _userRepository.GetUserByIdAsync(userId);

                if (user == null)
                {
                    return false;
                }

                            // 현재 비밀번호 확인
                bool isCurrentPasswordValid = _passwordHasher.VerifyPassword(
                    currentPassword, user.password_hash, user.salt);

                if (!isCurrentPasswordValid)
                {
                    return false;
                }

                            // 새 비밀번호 해싱
                var (passwordHash, salt) = _passwordHasher.HashPassword(newPassword);

                            // 사용자 정보 업데이트
                user.password_hash = passwordHash;
                user.salt = salt;

                bool updated = await _userRepository.UpdateAsync(user);

                if (updated)
                {
                                   // 모든 기기에서 로그아웃 처리
                    await _tokenService.RevokeAllUserTokensAsync(userId);
                }

                return updated;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
