using System.Reflection;
using WebServerProject.Data;
using WebServerProject.Models.DTOs;
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

        public HomeInitResponse InitHome(string userId)
        {
            var user = _playerRepo.GetUserById(userId);
            if (user == null)
                throw new Exception("User not found");

            // Entity -> PlayerDto 변환
            var player = new PlayerDto(
                user.userId,
                user.nickname,
                user.level,
                user.gold,
                user.diamonds,
                user.profileId,
                user.tutorialCompleted,
                user.createdAt
            );

            return new HomeInitResponse
            {
                playerInfo = player,
            };
        }
    }
}
