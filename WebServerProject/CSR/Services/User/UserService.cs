using SqlKata.Execution;
using System.Data;
using WebServerProject.CSR.Repositories.User;
using WebServerProject.Models.DTOs.Battle;
using WebServerProject.Models.DTOs.User;
using WebServerProject.Models.DTOs.UserEntity;
using WebServerProject.Models.Entities.UserEntity;

namespace WebServerProject.CSR.Services
{
    public interface IUserService
    {
        public Task<UserSafeDTO> GetUserAsync(int userId);
        public Task<UserSafeDTO> GetUserDetailAsync(int userId);
        public Task GrantBattleRewardAsync(BattleRewardDTO reward, QueryFactory? db = null, IDbTransaction? tx = null);
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

            userModel.stat = userStats;
            userModel.profile = userProfiles;
            userModel.resource = userResources;

            return userModel;
        }

        public async Task GrantBattleRewardAsync(BattleRewardDTO reward, QueryFactory? db = null, IDbTransaction? tx = null)
        {
            // 보상 지급
            var userResources = await _userRepository.GetUserResourcesByIdAsync(reward.userId);
            if(userResources == null)
            {
                throw new InvalidOperationException("유저 자원 정보가 없습니다.");
            }

            userResources.gold += reward.gold;
            await _userRepository.UpdateResourcesAsync(reward.userId, userResources, db, tx);

            // 경험치 지급 및 레벨업 처리
            var userStats = await _userRepository.GetUserStatsByIdAsync(reward.userId);
            if(userStats == null)
            {
                throw new InvalidOperationException("유저 통계 정보가 없습니다.");
            }

            userStats.exp += reward.exp;
            // TODO : 레벨업 처리
            // 현재는 경험치만 누적
            await _userRepository.UpdateStatsAsync(reward.userId, userStats, db, tx);
        }
    }
}
