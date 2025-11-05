using WebServerProject.Models.DTOs;
using WebServerProject.Repositories;

namespace WebServerProject.Services
{
    public interface IUserService
    {
        Task<UserSafeModel> GetUserInfoAsync(int userId);
    }
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository repo)
        {
            _userRepository = repo;
        }

        public async Task<UserSafeModel> GetUserInfoAsync(int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                return null;
            }

            user.Stats = await _userRepository.GetUserStatsByIdAsync(userId);
            if (user.Stats == null)
            {
                return null;
            }
            user.Profile = await _userRepository.GetUserProfilesByIdAsync(userId);
            if (user.Profile == null)
            {
                return null;
            }
            user.Resources = await _userRepository.GetUserResourcesByIdAsync(userId);
            if (user.Resources == null)
            {
                return null;
            }

            var userModel = UserSafeModel.FromUser(user, includeDetails: true);

            return userModel;
        }
    }
}
