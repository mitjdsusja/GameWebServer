using WebServerProject.Models.DTOs;
using WebServerProject.Repositories;

namespace WebServerProject.Services
{
    public class UserService
    {
        UserRepository _userRepo;

        public UserService(UserRepository repo)
        {
            _userRepo = repo;
        }

        public async Task<UserSafeModel> GetUserAsync(string userId)
        {
            
            return new UserSafeModel(
                
            );
        }

        public async Task<UserSafeModel> SetNicknameAsync(string userId, string newNickname)
        {
            return new UserSafeModel(

            );
        }
    }
}
