using WebServerProject.Data;

namespace WebServerProject.Repositories
{
    public class PlayerRepository
    {
        private readonly GameDbContext _db;

        public PlayerRepository(GameDbContext db)
        {
            _db = db;
        }

        public User CreateGuestUser()
        {
            var user = new User
            {
                userId = Guid.NewGuid().ToString(),
                nickname = "Guest"
            };
            _db.users.Add(user);
            _db.SaveChanges();
            return user;
        }

        public bool UserExists(string userId)
        {
            return _db.users.Any(u => u.userId == userId);
        }

        public User GetUserById(string userId)
        {
            var user = _db.users.FirstOrDefault(u => u.userId == userId);

            return user;
        }

        public User SetNickname(string userId, string newNickname)
        {
            var user = _db.users.FirstOrDefault(u => u.userId == userId);
            if (user == null)
                return null;

            user.nickname = newNickname;
            _db.SaveChanges();
            return user;
        }
    }
}
