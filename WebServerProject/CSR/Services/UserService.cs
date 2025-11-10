using WebServerProject.CSR.Repositories;
using WebServerProject.Models.DTOs.User;

namespace WebServerProject.CSR.Services
{
    public interface IUserService
    {
        Task<(UserSafeDTO, UserStatsDTO, UserProfilesDTO, UserResourcesDTO)?> GetUserInfoAsync(int userId);
    }
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository repo)
        {
            _userRepository = repo;
        }

        public async Task<(UserSafeDTO, UserStatsDTO, UserProfilesDTO, UserResourcesDTO)?> GetUserInfoAsync(int userId)
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

            var userModel = UserSafeDTO.FromUser(user);
            var userStats = UserStatsDTO.FromUserStats(stats);
            var userProfiles = UserProfilesDTO.FromUserProfiles(profile);
            var userResources = UserResourcesDTO.FromUserResources(resources);

            return (userModel, userStats, userProfiles, userResources);
        }
    }
}
