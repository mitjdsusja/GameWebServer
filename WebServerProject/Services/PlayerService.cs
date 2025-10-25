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

        public async Task<PlayerDto> GetPlayerInfoAsync(string userId)
        {
            var user = await _repo.GetUserByIdAsync(userId);
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

        public async Task<PlayerDto> SetNicknameAsync(string userId, string newNickname)
        {
            var user = await _repo.SetNicknameAsync(userId, newNickname);
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
