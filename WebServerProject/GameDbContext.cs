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
                entity.Property(e => e.UserId)
                      .HasColumnType("varchar(255)")
                      .IsRequired();

                entity.Property(e => e.Nickname)
                      .HasColumnType("varchar(255)")
                      .HasDefaultValue("Guest")
                      .IsRequired();

                // 나머지 기본값 설정
                entity.Property(e => e.Level).HasDefaultValue(1).IsRequired();
                entity.Property(e => e.Gold).HasDefaultValue(0).IsRequired();
                entity.Property(e => e.Diamonds).HasDefaultValue(0).IsRequired();
                entity.Property(e => e.ProfileId).HasDefaultValue(1).IsRequired();
                entity.Property(e => e.TutorialCompleted).HasDefaultValue(false).IsRequired();
            });
        }

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
