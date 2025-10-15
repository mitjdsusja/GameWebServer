using Microsoft.EntityFrameworkCore;

namespace WebServerProject.Data
{
    public class GameDbContext : DbContext
    {
        public GameDbContext(DbContextOptions<GameDbContext> options) : base(options) { }

        // 예: 유저 테이블
        public DbSet<User> Users { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public string UserId { get; set; } // 예: 게스트 UID
        public string Nickname { get; set; } = "Guest";
        public int Level { get; set; } = 1;
        public int Gold { get; set; } = 0;
        public int Diamonds { get; set; } = 0;
        public int ProfileId { get; set; } = 1;
        public bool TutorialCompleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
