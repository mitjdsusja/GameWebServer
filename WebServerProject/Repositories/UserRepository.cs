using Microsoft.EntityFrameworkCore;
using WebServerProject.Data;
using WebServerProject.Models.Entities;

namespace WebServerProject.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(int userId);
        Task<User> GetByUsernameAsync(string username);
        Task<User> GetByEmailAsync(string email);
        Task<int> CreateAsync(User user);
        Task<bool> UpdateAsync(User user);
        Task<bool> UpdateLastLoginAsync(int userId, DateTime loginTime);
    }
    public class UserRepository : IUserRepository
    {
        private readonly AuthDbContext _db;

        public UserRepository(AuthDbContext db)
        {
            _db = db;
        }

        public async Task<User> GetByIdAsync(int userId)
        {
            return await _db.Users.FindAsync(userId);
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.UserName == username);
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<int> CreateAsync(User user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return user.Id;
        }

        public async Task<bool> UpdateAsync(User user)
        {
            _db.Users.Update(user);
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateLastLoginAsync(int userId, DateTime loginTime)
        {
            var user = new User { Id = userId, LastLoginAt = loginTime };
            _db.Users.Attach(user);
            _db.Entry(user).Property(u => u.LastLoginAt).IsModified = true;

            return await _db.SaveChangesAsync() > 0;
        }
    }
}
