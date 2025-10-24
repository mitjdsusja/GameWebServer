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
    }
}
