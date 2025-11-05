using Microsoft.EntityFrameworkCore;
using WebServerProject.Data;
using WebServerProject.Models.Entities.User;

namespace WebServerProject.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int userId);
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetFullByIdAsync(int userId);
        Task<int> CreateAsync(User user);
        Task<bool> UpdateAsync(User user);
        Task<bool> UpdateLastLoginAsync(User user, DateTime loginTime);
    }
    public class UserRepository : IUserRepository
    {
        private readonly UserDbContext _db;

        public UserRepository(UserDbContext db)
        {
            _db = db;
        }

        public async Task<User?> GetByIdAsync(int userId)
        { 
            return await _db.Users.FindAsync(userId);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.UserName == username);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetFullByIdAsync(int userId)
        {
            return await _db.Users
                .Include(u => u.Stats)
                .Include(u => u.Resources)
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.Id == userId);
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

        public async Task<bool> UpdateLastLoginAsync(User user, DateTime loginTime)
        {
            user.LastLoginAt = loginTime;
            return await _db.SaveChangesAsync() > 0;
        }

    }
}
