using SqlKata.Execution;
using WebServerProject.Models.Entities.User;

namespace WebServerProject.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetUserByIdAsync(int userId);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User?> GetUserByEmailAsync(string email);
        Task<UserStats> GetUserStatsByIdAsync(int userId);
        Task<UserProfiles> GetUserProfilesByIdAsync(int userId);
        Task<UserResources> GetUserResourcesByIdAsync(int userId);
        Task<int> CreateAsync(User user);
        Task<bool> UpdateAsync(User user);
        Task<bool> UpdateLastLoginAsync(int userId, DateTime loginTime);
    }
    public class UserRepository : IUserRepository
    {
        private readonly QueryFactory _db;

        public UserRepository(QueryFactory db)
        {
            _db = db;
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _db.Query("users")
                            .Where("id", userId)
                            .FirstOrDefaultAsync<User>();
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _db.Query("users")
                            .Where("username", username)
                            .FirstOrDefaultAsync<User>();
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _db.Query("users")
                            .Where("email", email)
                            .FirstOrDefaultAsync<User>();
        }

        public async Task<UserStats> GetUserStatsByIdAsync(int userId)
        {
            return await _db.Query("user_stats")
                            .Where("userId", userId)
                            .FirstOrDefaultAsync<UserStats>();
        }

        public async Task<UserProfiles> GetUserProfilesByIdAsync(int userId)
        {
            return await _db.Query("user_profiles")
                            .Where("userId", userId)
                            .FirstOrDefaultAsync<UserProfiles>();
        }

        public async Task<UserResources> GetUserResourcesByIdAsync(int userId)
        {
            return await _db.Query("user_resources")
                            .Where("userId", userId)
                            .FirstOrDefaultAsync<UserResources>();
        }

        public async Task<int> CreateAsync(User user)
        {
            var id = await _db.Query("users").InsertGetIdAsync<int>(new
            {
                username = user.UserName,
                email = user.Email,
                password = user.PasswordHash,
                created_at = DateTime.UtcNow,
                last_login_at = user.LastLoginAt
            });
            return id;
        }

        public async Task<bool> UpdateAsync(User user)
        {
            var affected = await _db.Query("users")
                .Where("id", user.Id)
                .UpdateAsync(new
                {
                    username = user.UserName,
                    email = user.Email,
                    password = user.PasswordHash,
                    last_login_at = user.LastLoginAt
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
