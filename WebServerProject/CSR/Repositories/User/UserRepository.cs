using SqlKata.Execution;
using UserEntity = WebServerProject.Models.Entities.UserEntity.User;
using UserStateEntity = WebServerProject.Models.Entities.UserEntity.UserStats;
using UserProfileEntity = WebServerProject.Models.Entities.UserEntity.UserProfiles;
using UserResourcesEntity = WebServerProject.Models.Entities.UserEntity.UserResources;

namespace WebServerProject.CSR.Repositories.User
{
    public interface IUserRepository
    {
        Task<UserEntity?> GetUserByIdAsync(int userId);
        Task<UserEntity?> GetUserByUsernameAsync(string username);
        Task<UserEntity?> GetUserByEmailAsync(string email);
        Task<UserStateEntity?> GetUserStatsByIdAsync(int userId);
        Task<UserProfileEntity?> GetUserProfilesByIdAsync(int userId);
        Task<UserResourcesEntity?> GetUserResourcesByIdAsync(int userId);
        Task<int> CreateAsync(UserEntity user);
        Task<bool> UpdateAsync(UserEntity user);
        Task<bool> UpdateLastLoginAsync(int userId, DateTime loginTime);
    }
    public class UserRepository : IUserRepository
    {
        private readonly QueryFactory _db;

        public UserRepository(QueryFactory db)
        {
            _db = db;
        }

        public async Task<UserEntity?> GetUserByIdAsync(int userId)
        {
            return await _db.Query("users")
                            .Where("id", userId)
                            .FirstOrDefaultAsync<UserEntity>();
        }

        public async Task<UserEntity?> GetUserByUsernameAsync(string username)
        {
            return await _db.Query("users")
                            .Where("username", username)
                            .FirstOrDefaultAsync<UserEntity>();
        }

        public async Task<UserEntity?> GetUserByEmailAsync(string email)
        {
            return await _db.Query("users")
                            .Where("email", email)
                            .FirstOrDefaultAsync<UserEntity>();
        }

        public async Task<UserStateEntity?> GetUserStatsByIdAsync(int userId)
        {
            return await _db.Query("user_stats")
                            .Where("user_id", userId)
                            .FirstOrDefaultAsync<UserStateEntity>();
        }

        public async Task<UserProfileEntity?> GetUserProfilesByIdAsync(int userId)
        {
            return await _db.Query("user_profiles")
                            .Where("user_id", userId)
                            .FirstOrDefaultAsync<UserProfileEntity>();
        }

        public async Task<UserResourcesEntity?> GetUserResourcesByIdAsync(int userId)
        {
            return await _db.Query("user_resources")
                            .Where("user_id", userId)
                            .FirstOrDefaultAsync<UserResourcesEntity>();
        }

        public async Task<int> CreateAsync(UserEntity user)
        {
            var id = await _db.Query("users").InsertGetIdAsync<int>(new
            {
                user.username,
                user.email,
                password = user.password_hash,
                created_at = DateTime.UtcNow,
                user.last_login_at
            });
            return id;
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
    }
}
