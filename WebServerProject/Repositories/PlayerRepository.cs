using Microsoft.EntityFrameworkCore;
using WebServerProject.Data;
using WebServerProject.Models;

namespace WebServerProject.Repositories
{
    public class PlayerRepository
    {
        private readonly GameDbContext _db;

        public PlayerRepository(GameDbContext db)
        {
            _db = db;
        }

        public async Task<User> CreateGuestUserAsync()
        {
            var user = new User
            {
                userId = Guid.NewGuid().ToString(),
                nickname = "Guest"
            };
            _db.users.Add(user);
            await _db.SaveChangesAsync();
            return user;
        }

        public async Task<bool> UserExistsAsync(string userId)
        {
            return await _db.users.AnyAsync(u => u.userId == userId);
        }

        public async Task<User> GetUserByIdAsync(string userId)
        {
            var user = await _db.users.FirstOrDefaultAsync(u => u.userId == userId);

            return user;
        }

        public async Task<User> SetNicknameAsync(string userId, string newNickname)
        {
            var user = await _db.users.FirstOrDefaultAsync(u => u.userId == userId);
            if (user == null)
                return null;

            user.nickname = newNickname;
            await _db.SaveChangesAsync();
            return user;
        }
    }
}
