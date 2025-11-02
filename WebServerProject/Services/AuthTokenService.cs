using System.Security.Cryptography;
using WebServerProject.Models.Auth;
using WebServerProject.Models.Entities;

namespace WebServerProject.Services
{
    public interface IAuthTokenService
    {
        Task<AuthToken> CreateTokenAsync(User user);
        Task<AuthToken> GetTokenAsync(string token);
        Task<bool> ValidateTokenAsync(string token);
        Task<bool> RevokeTokenAsync(string token);
        Task<IEnumerable<AuthToken>> GetUserTokensAsync(int userId);
        Task<bool> RevokeAllUserTokensAsync(int userId);
    }

    public class AuthTokenService : IAuthTokenService
    {
        private readonly TimeSpan _tokenExpiry = TimeSpan.FromDays(7); // 토큰 기본 유효 기간: 7일

        public async Task<AuthToken> CreateTokenAsync(User user)
        {
            // 고유한 토큰 생성
            string tokenString = GenerateUniqueToken();

            // 토큰 객체 생성
            var token = new AuthToken
            {
                Token = tokenString,
                UserId = user.Id,
                Username = user.UserName,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.Add(_tokenExpiry)
            };

            // TODO : Redis 또는 데이터베이스에 토큰 저장 로직 추가
            //

            return await Task.FromResult(token);
        }

        public Task<AuthToken> GetTokenAsync(string token)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AuthToken>> GetUserTokensAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RevokeAllUserTokensAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RevokeTokenAsync(string token)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ValidateTokenAsync(string token)
        {
            throw new NotImplementedException();
        }

        private string GenerateUniqueToken()
        {
            // 128비트(16바이트) 랜덤 값 생성
            byte[] randomBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }

            // Base64url 형식(URL 안전한 Base64)으로 인코딩
            string base64 = Convert.ToBase64String(randomBytes);
            string base64url = base64.Replace('+', '-').Replace('/', '_').Replace("=", "");

            return base64url;
        }
    }
}
