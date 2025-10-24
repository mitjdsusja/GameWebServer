using WebServerProject.Data;
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

        public User GuestLogin()
        {
            // 필요하다면 추가 검증 로직 가능
            return _repo.CreateGuestUser();
        }

        public User GoogleLogin(string email)
        {
            // Google 로그인 처리 로직 (생략)
            throw new NotImplementedException();
        }
       
        public bool CheckUID(string userId)
        {
            // UID 확인 로직 (생략)
            throw new NotImplementedException();
        }
    }
}
