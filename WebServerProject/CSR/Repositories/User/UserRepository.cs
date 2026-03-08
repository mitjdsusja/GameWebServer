using Dapper;
using SqlKata.Execution;
using System.Data;
using UserEntity = WebServerProject.Models.Entities.UserEntity.User;
using UserProfileEntity = WebServerProject.Models.Entities.UserEntity.UserProfiles;
using UserResourcesEntity = WebServerProject.Models.Entities.UserEntity.UserResources;
using UserStateEntity = WebServerProject.Models.Entities.UserEntity.UserStats;

namespace WebServerProject.CSR.Repositories.User
{
    public interface IUserRepository
    {
        Task<UserEntity?> GetUserByIdAsync(int userId, IDbTransaction? tx = null);
        Task<UserEntity?> GetUserByUsernameAsync(string username, IDbTransaction? tx = null);
        Task<UserEntity?> GetUserByEmailAsync(string email, IDbTransaction? tx = null);
        Task<UserStateEntity?> GetUserStatsByIdAsync(int userId, IDbTransaction? tx = null);
        Task<UserProfileEntity?> GetUserProfilesByIdAsync(int userId, IDbTransaction? tx = null);
        Task<UserResourcesEntity?> GetUserResourcesByIdAsync(int userId, IDbTransaction? tx = null);
        Task<UserResourcesEntity?> GetUserResourcesForUpdateAsync(int userId, IDbTransaction? tx = null);
        Task<int> CreateUserAsync(UserEntity user, IDbTransaction? tx = null);
        Task CreateUserStatsAsync(int userId, IDbTransaction? tx = null);
        Task CreateUserProfilesAsync(int userId, string nickname, IDbTransaction? tx = null);
        Task CreateUserResourcesAsync(int userId, IDbTransaction? tx = null);
        Task<bool> UpdateAsync(UserEntity user, IDbTransaction? tx = null);
        Task<bool> UpdateLastLoginAsync(int userId, DateTime loginTime, IDbTransaction? tx = null);
        Task<bool> UpdateResourcesAsync(int userId, UserResourcesEntity resources, IDbTransaction? tx = null);
            
        Task<bool> UpdateStatsAsync(int userId, UserStateEntity stats, IDbTransaction? tx = null);
    }
    public class UserRepository : IUserRepository
    {
        private readonly QueryFactory _db;

        public UserRepository(QueryFactory db)
        {
            _db = db;
        }

        public async Task<UserEntity?> GetUserByIdAsync(int userId, IDbTransaction? tx = null)
        {
            return await _db.Query("users")
                            .Where("id", userId)
                            .FirstOrDefaultAsync<UserEntity>(tx);
        }

        public async Task<UserEntity?> GetUserByUsernameAsync(string username, IDbTransaction? tx = null)
        {
            return await _db.Query("users")
                            .Where("username", username)
                            .FirstOrDefaultAsync<UserEntity>(tx);
        }

        public async Task<UserEntity?> GetUserByEmailAsync(string email, IDbTransaction? tx = null)
        {
            return await _db.Query("users")
                            .Where("email", email)
                            .FirstOrDefaultAsync<UserEntity>(tx);
        }

        public async Task<UserStateEntity?> GetUserStatsByIdAsync(int userId, IDbTransaction? tx = null)
        {
            return await _db.Query("user_stats")
                            .Where("user_id", userId)
                            .FirstOrDefaultAsync<UserStateEntity>(tx);
        }

        public async Task<UserProfileEntity?> GetUserProfilesByIdAsync(int userId, IDbTransaction? tx = null)
        {
            return await _db.Query("user_profiles")
                            .Where("user_id", userId)
                            .FirstOrDefaultAsync<UserProfileEntity>();
        }

        public async Task<UserResourcesEntity?> GetUserResourcesByIdAsync(int userId, IDbTransaction? tx = null)
        {
            return await _db.Query("user_resources")
                            .Where("user_id", userId)
                            .FirstOrDefaultAsync<UserResourcesEntity>(tx);
        }

        public async Task<int> CreateUserAsync(UserEntity user, IDbTransaction? tx = null)
        {
            var id = await _db.Query("users").InsertGetIdAsync<int>(new
            {
                user.username,
                user.email,
                user.password_hash,
                user.salt,
                created_at = DateTime.UtcNow,
                user.last_login_at
            }, tx);
            return id;
        }

        public async Task CreateUserStatsAsync(int userId, IDbTransaction? tx = null)
        {
            await _db.Query("user_stats").InsertAsync(new
            {
                user_id = userId
            }, tx);
        }
        public async Task CreateUserProfilesAsync(int userId, string nickname, IDbTransaction? tx = null)
        {
            await _db.Query("user_profiles").InsertAsync(new
            {
                user_id = userId,
                nickname = nickname
            }, tx);
        }

        public async Task CreateUserResourcesAsync(int userId, IDbTransaction? tx = null)
        {
            await _db.Query("user_resources").InsertAsync(new
            {
                user_id = userId,
            }, tx);
        }

        public async Task<bool> UpdateAsync(UserEntity user, IDbTransaction? tx = null)
        {
            var affected = await _db.Query("users")
                .Where("id", user.id)
                .UpdateAsync(new
                {
                    user.username,
                    user.email,
                    password = user.password_hash,
                    user.last_login_at
                });

            return affected > 0;
        }

        public async Task<bool> UpdateLastLoginAsync(int userId, DateTime loginTime, IDbTransaction? tx = null)
        {
            var affected = await _db.Query("users")
                .Where("id", userId)
                .UpdateAsync(new { last_login_at = loginTime });

            return affected > 0;
        }

        public async Task<bool> UpdateResourcesAsync(int userId, UserResourcesEntity resources, QueryFactory? db = null, IDbTransaction? tx = null)
        {
            var result = await _db.Query("user_resources")
                                  .Where ("user_id", userId)
                                  .UpdateAsync(new
                                  {
                                      gold = resources.gold,
                                      diamond = resources.diamond,
                                  }, tx);

            return result == 1;
        }

        public async Task<bool> UpdateStatsAsync(int userId, UserStateEntity stats, QueryFactory? db = null, IDbTransaction? tx = null)
        {
            var result = await _db.Query("user_stats")
                                  .Where("user_id", userId)
                                  .UpdateAsync(new
                                  {
                                      level = stats.level,
                                      exp = stats.exp,
                                  }, tx);

            return result == 1;
        }

        public async Task<UserResourcesEntity?> GetUserResourcesForUpdateAsync(int userId, IDbTransaction? tx = null)
        {
            var sql = "SELECT * FROM user_resources WHERE user_id = @userId FOR UPDATE";
            
            return await _db.Connection.QueryFirstOrDefaultAsync<UserResourcesEntity>(
                sql,
                new { userId },
                tx
            );
        }

        public Task<bool> UpdateResourcesAsync(int userId, UserResourcesEntity resources, IDbTransaction? tx = null)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateStatsAsync(int userId, UserStateEntity stats, IDbTransaction? tx = null)
        {
            throw new NotImplementedException();
        }
    }
}
