using WebServerProject.Models.DTOs;
using WebServerProject.Repositories;

namespace WebServerProject.Services
{
    public interface IUserService
    {
        Task<(UserSafeModel, UserStatsModel, UserProfilesModel, UserResourcesModel)?> GetUserInfoAsync(int userId);
    }
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository repo)
        {
            _userRepository = repo;
        }

        public async Task<(UserSafeModel, UserStatsModel, UserProfilesModel, UserResourcesModel)?> GetUserInfoAsync(int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                return null;

            var stats = await _userRepository.GetUserStatsByIdAsync(userId);
            var profile = await _userRepository.GetUserProfilesByIdAsync(userId);
            var resources = await _userRepository.GetUserResourcesByIdAsync(userId);

            // 하나라도 없으면 null 반환
            if (stats == null || profile == null || resources == null)
                return null;

            var userModel = UserSafeModel.FromUser(user);
            var userStats = UserStatsModel.FromUserStats(stats);
            var userProfiles = UserProfilesModel.FromUserProfiles(profile);
            var userResources = UserResourcesModel.FromUserResources(resources);

            return (userModel, userStats, userProfiles, userResources);
        }
    }
}
