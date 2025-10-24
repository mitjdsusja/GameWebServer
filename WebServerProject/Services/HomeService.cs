using WebServerProject.Data;
using WebServerProject.Repositories;

namespace WebServerProject.Services
{
    public class HomeService
    {
        private readonly PlayerRepository _playerRepo;

        public HomeService(PlayerRepository playerRepo)
        {
            _playerRepo = playerRepo;
        }

        public class HomeData
        {
            public User user { get; set; }

            // 추가적인 홈 초기화 데이터 필드
        }

        public HomeData InitHome(string userId)
        {
            var user = _playerRepo.GetUserById(userId);
            if (user == null)
                throw new Exception("User not found");

            return new HomeData
            {
                user = user
            };
        }
    }
}
