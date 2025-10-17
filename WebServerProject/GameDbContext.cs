using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace WebServerProject.Data
{
    public class GameDbContext : DbContext
    {
        public GameDbContext(DbContextOptions<GameDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                // 문자열 컬럼을 varchar로 지정하고 기본값 설정
                entity.Property(e => e.userId)
                      .HasColumnType("varchar(255)")
                      .IsRequired();

                entity.Property(e => e.nickname)
                      .HasColumnType("varchar(255)")
                      .HasDefaultValue("Guest")
                      .IsRequired();

                // 나머지 기본값 설정
                entity.Property(e => e.level).HasDefaultValue(1).IsRequired();
                entity.Property(e => e.gold).HasDefaultValue(0).IsRequired();
                entity.Property(e => e.diamonds).HasDefaultValue(0).IsRequired();
                entity.Property(e => e.profileId).HasDefaultValue(1).IsRequired();
                entity.Property(e => e.tutorialCompleted).HasDefaultValue(false).IsRequired();
            });
        }

        // 예: 유저 테이블
        public DbSet<User> users { get; set; }
    }

    public class User
    {
        public int id { get; set; }
        public string userId { get; set; } // 예: 게스트 UID
        public string nickname { get; set; } = "Guest";
        public int level { get; set; } = 1;
        public int gold { get; set; } = 0;
        public int diamonds { get; set; } = 0;
        public int profileId { get; set; } = 1;
        public bool tutorialCompleted { get; set; } = false;
        public DateTime createdAt { get; set; } = DateTime.UtcNow;
    }
}
