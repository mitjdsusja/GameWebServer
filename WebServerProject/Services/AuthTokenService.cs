using System.Security.Cryptography;
using WebServerProject.Models.Auth;
using WebServerProject.Models.Entities;

namespace WebServerProject.Services
{
    public interface IAuthTokenService
    {
        Task<AuthToken> CreateTokenAsync(User user, string deviceId);
        Task<AuthToken> GetTokenAsync(string token);
        Task<bool> ValidateTokenAsync(string token);
        Task<bool> RevokeTokenAsync(string token);
        Task<IEnumerable<AuthToken>> GetUserTokensAsync(int userId);
        Task<bool> RevokeAllUserTokensAsync(int userId);
    }

    public class AuthTokenService : IAuthTokenService
    {
        public Task<AuthToken> CreateTokenAsync(User user, string deviceId)
        {
            throw new NotImplementedException();
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
    }
}
