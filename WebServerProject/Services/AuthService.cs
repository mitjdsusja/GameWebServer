using WebServerProject.Data;
using WebServerProject.Models;
using WebServerProject.Repositories;

namespace WebServerProject.Services
{
    public class AuthService
    {
        private readonly PlayerRepository _repo;

        public AuthService(PlayerRepository repo)
        {
            _repo = repo;
        }

        public async Task<User> GuestLoginAsync()
        {
            // 필요하다면 추가 검증 로직 가능
            return await _repo.CreateGuestUserAsync();
        }

        public async Task<User> GoogleLoginAsync(string email)
        {
            // Google 로그인 처리 로직 (생략)
            throw new NotImplementedException();
        }
       
        public async Task<bool> CheckUIDAsync(string userId)
        {
            bool result = await _repo.UserExistsAsync(userId);

            return result;
        }
    }
}
