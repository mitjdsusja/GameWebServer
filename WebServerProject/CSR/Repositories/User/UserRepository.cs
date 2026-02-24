using Dapper;
using SqlKata.Execution;
using System.Data;
using WebServerProject.Models.Entities.UserEntity;
using UserEntity = WebServerProject.Models.Entities.UserEntity.User;
using UserProfileEntity = WebServerProject.Models.Entities.UserEntity.UserProfiles;
using UserResourcesEntity = WebServerProject.Models.Entities.UserEntity.UserResources;
using UserStateEntity = WebServerProject.Models.Entities.UserEntity.UserStats;

namespace WebServerProject.CSR.Repositories.User
{
    public interface IUserRepository
    {
        Task<UserEntity?> GetUserByIdAsync(int userId, QueryFactory? db = null, IDbTransaction? tx = null);
        Task<UserEntity?> GetUserByUsernameAsync(string username, QueryFactory? db = null, IDbTransaction? tx = null);
        Task<UserEntity?> GetUserByEmailAsync(string email, QueryFactory? db = null, IDbTransaction? tx = null);
        Task<UserStateEntity?> GetUserStatsByIdAsync(int userId, QueryFactory? db = null, IDbTransaction? tx = null);
        Task<UserProfileEntity?> GetUserProfilesByIdAsync(int userId);
        Task<UserResourcesEntity?> GetUserResourcesAsync(int userId, QueryFactory? db = null, IDbTransaction? tx = null);
        Task<UserResourcesEntity?> GetUserResourcesForUpdateAsync(int userId, QueryFactory? db = null, IDbTransaction? tx = null);
        Task<int> CreateUserAsync(UserEntity user, QueryFactory? db = null, IDbTransaction? tx = null);
        Task CreateUserStatsAsync(int userId, QueryFactory? db = null, IDbTransaction? tx = null);
        Task CreateUserProfilesAsync(int userId, string nickname, QueryFactory? db = null, IDbTransaction? tx = null);
        Task CreateUserResourcesAsync(int userId, QueryFactory? db = null, IDbTransaction? tx = null);
        Task<bool> UpdateAsync(UserEntity user);
        Task<bool> UpdateLastLoginAsync(int userId, DateTime loginTime);
        Task<bool> UpdateResourcesAsync(int userId, UserResourcesEntity resources, QueryFactory? db = null, IDbTransaction? tx = null);
            
        Task<bool> UpdateStatsAsync(int userId, UserStateEntity stats, QueryFactory? db = null, IDbTransaction? tx = null);
    }
    public class UserRepository : IUserRepository
    {
        private readonly QueryFactory _db;

        public UserRepository(QueryFactory db)
        {
            _db = db;
        }

        public async Task<UserEntity?> GetUserByIdAsync(int userId, QueryFactory? db = null, IDbTransaction? tx = null)
        {
            var q = db ?? _db;

            return await q.Query("users")
                            .Where("id", userId)
                            .FirstOrDefaultAsync<UserEntity>(tx);
        }

        public async Task<UserEntity?> GetUserByUsernameAsync(string username, QueryFactory? db = null, IDbTransaction? tx = null)
        {
            var q = db ?? _db;

            return await q.Query("users")
                            .Where("username", username)
                            .FirstOrDefaultAsync<UserEntity>(tx);
        }

        public async Task<UserEntity?> GetUserByEmailAsync(string email, QueryFactory? db = null, IDbTransaction? tx = null)
        {
            var q = db ?? _db;

            return await q.Query("users")
                            .Where("email", email)
                            .FirstOrDefaultAsync<UserEntity>(tx);
        }

        public async Task<UserStateEntity?> GetUserStatsByIdAsync(int userId, QueryFactory? db = null, IDbTransaction? tx = null)
        {
            var q = db ?? _db;

            return await q.Query("user_stats")
                            .Where("user_id", userId)
                            .FirstOrDefaultAsync<UserStateEntity>(tx);
        }

        public async Task<UserProfileEntity?> GetUserProfilesByIdAsync(int userId)
        {
            return await _db.Query("user_profiles")
                            .Where("user_id", userId)
                            .FirstOrDefaultAsync<UserProfileEntity>();
        }

        public async Task<int> CreateUserAsync(UserEntity user, QueryFactory? db = null, IDbTransaction? tx = null)
        {
            var q = db ?? _db;

            var id = await q.Query("users").InsertGetIdAsync<int>(new
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

        public async Task CreateUserStatsAsync(int userId, QueryFactory? db = null, IDbTransaction? tx = null)
        {
            var q = db ?? _db;

            await q.Query("user_stats").InsertAsync(new
            {
                user_id = userId
            }, tx);
        }
        public async Task CreateUserProfilesAsync(int userId, string nickname, QueryFactory? db = null, IDbTransaction? tx = null)
        {
            var q = db ?? _db;

            await q.Query("user_profiles").InsertAsync(new
            {
                user_id = userId,
                nickname = nickname
            }, tx);
        }

        public async Task CreateUserResourcesAsync(int userId, QueryFactory? db = null, IDbTransaction? tx = null)
        {
            var q = db ?? _db;

            await q.Query("user_resources").InsertAsync(new
            {
                user_id = userId,
            }, tx);
        }

        public async Task<bool> UpdateAsync(UserEntity user)
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

        public async Task<bool> UpdateLastLoginAsync(int userId, DateTime loginTime)
        {
            var affected = await _db.Query("users")
                .Where("id", userId)
                .UpdateAsync(new { last_login_at = loginTime });

            return affected > 0;
        }

        public async Task<bool> UpdateResourcesAsync(int userId, UserResourcesEntity resources, QueryFactory? db = null, IDbTransaction? tx = null)
        {
            var q = db ?? _db;

            var result = await q.Query("user_resources")
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
            var q = db ?? _db;

            var result = await q.Query("user_stats")
                                  .Where("user_id", userId)
                                  .UpdateAsync(new
                                  {
                                      level = stats.level,
                                      exp = stats.exp,
                                  }, tx);

            return result == 1;
        }

        public async Task<UserResourcesEntity?> GetUserResourcesAsync(int userId, QueryFactory? db = null, IDbTransaction? tx = null)
        {
            var q = db ?? _db;

            return await q.Query("user_resources")
                            .Where("user_id", userId)
                            .FirstOrDefaultAsync<UserResourcesEntity>(tx);
        }

        public async Task<UserResourcesEntity?> GetUserResourcesForUpdateAsync(int userId, QueryFactory? db = null, IDbTransaction? tx = null)
        {
            var q = db ?? _db;

            string sql = "SELECT * FROM user_resources WHERE user_id = @UserId FOR UPDATE";

            return await q.Connection.QueryFirstOrDefaultAsync<UserResourcesEntity>(
                sql,
                new { UserId = userId },
                tx
            );
        }
    }
}
