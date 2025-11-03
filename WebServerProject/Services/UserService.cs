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
            var user = await _userRepository.GetFullByIdAsync(userId);

            if (user == null)
            {
                return null;
            }

            var userModel = UserSafeModel.FromUser(user, includeDetails: true);

            return userModel;
        }
    }
}
