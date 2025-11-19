using WebServerProject.CSR.Repositories.User;
using WebServerProject.Models.DTOs.User;
using WebServerProject.Models.DTOs.UserEntity;

namespace WebServerProject.CSR.Services
{
    public interface IUserService
    {
        public Task<UserSafeDTO> GetUserAsync(int userId);
        public Task<UserSafeDTO> GetUserDetailAsync(int userId);
    }
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository repo)
        {
            _userRepository = repo;
        }

        public async Task<UserSafeDTO> GetUserAsync(int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if(user == null)
            {
                throw new InvalidOperationException("유저 정보가 없습니다.");
            }

            UserSafeDTO userDTO = new UserSafeDTO();
            userDTO = UserSafeDTO.FromUser(user);

            return userDTO;
        }

        public async Task<UserSafeDTO> GetUserDetailAsync(int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException("유저 정보가 없습니다.");
            }

            var stats = await _userRepository.GetUserStatsByIdAsync(userId);
            var profile = await _userRepository.GetUserProfilesByIdAsync(userId);
            var resources = await _userRepository.GetUserResourcesByIdAsync(userId);

            // 하나라도 없으면 예외 처리
            if (stats == null || profile == null || resources == null)
            {
                throw new InvalidOperationException("유저 상세 정보가 없습니다.");
            }

            var userModel = UserSafeDTO.FromUser(user);
            var userStats = UserStatsDTO.FromUserStats(stats);
            var userProfiles = UserProfilesDTO.FromUserProfiles(profile);
            var userResources = UserResourcesDTO.FromUserResources(resources);

            userModel.stats = userStats;
            userModel.profiles = userProfiles;
            userModel.resources = userResources;

            return userModel;
        }
    }
}
