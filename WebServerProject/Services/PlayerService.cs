using WebServerProject.Data;
using WebServerProject.Models.DTOs;
using WebServerProject.Repositories;

namespace WebServerProject.Services
{
    public class PlayerService
    {
        PlayerRepository _repo;

        public PlayerService(PlayerRepository repo)
        {
            _repo = repo;
        }

        public PlayerDto GetPlayerInfo(string userId)
        {
            var user = _repo.GetUserById(userId);
            if (user == null)
                throw new Exception("User not found");

            return new PlayerDto(
                user.userId,
                user.nickname,
                user.level,
                user.gold,
                user.diamonds,
                user.profileId,
                user.tutorialCompleted,
                user.createdAt
            );
        }
    }
}
