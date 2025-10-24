using WebServerProject.Data;
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

        public User GetPlayerInfo(string userId)
        {
            var user = _repo.GetUserById(userId);
            if (user == null)
                throw new Exception("User not found");

            return user;
        }
    }
}
