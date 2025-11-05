using Microsoft.EntityFrameworkCore;
using WebServerProject.Models.Entities.User;

namespace WebServerProject.Data
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User 엔티티 구성
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.UserName)
                      .HasColumnType("varchar(50)")
                      .IsRequired();

                entity.Property(e => e.Email)
                      .HasColumnType("varchar(100)")
                      .IsRequired();

                entity.Property(e => e.PasswordHash)
                      .HasColumnType("varchar(255)")
                      .IsRequired();

                entity.Property(e => e.Salt)
                      .HasColumnType("varchar(50)")
                      .IsRequired();

                entity.Property(e => e.CreatedAt)
                      .HasColumnType("datetime")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP")
                      .IsRequired();

                entity.Property(e => e.LastLoginAt)
                      .HasColumnType("datetime")
                      .IsRequired(false);

                entity.Property(e => e.Status)
                      .HasColumnType("enum('active', 'suspended', 'banned')")
                      .HasDefaultValue("active")
                      .IsRequired();

                // UNIQUE + INDEX 동일하게 구성
                entity.HasIndex(e => e.UserName)
                      .HasDatabaseName("idx_username")
                      .IsUnique();

                entity.HasIndex(e => e.Email)
                      .HasDatabaseName("idx_email")
                      .IsUnique();
            });

            modelBuilder.Entity<UserStats>(entity =>
            {
                entity.ToTable("user_stats");

                entity.HasKey(e => e.UserId);

                entity.Property(e => e.Level)
                      .HasColumnType("int")
                      .HasDefaultValue(1)
                      .IsRequired();

                entity.Property(e => e.Exp)
                      .HasColumnType("int")
                      .HasDefaultValue(0)
                      .IsRequired();
            });

            modelBuilder.Entity<UserResources>(entity =>
            {
                entity.ToTable("user_resources");

                entity.HasKey(e => e.UserId);

                entity.Property(e => e.Golds)
                      .HasColumnType("int")
                      .HasDefaultValue(0)
                      .IsRequired();

                entity.Property(e => e.Diamonds)
                        .HasColumnType("int")
                        .HasDefaultValue(0)
                        .IsRequired();
            });

            modelBuilder.Entity<UserProfiles>(entity =>
            {
                entity.ToTable("user_profiles");

                entity.HasKey(e => e.UserId);

                entity.Property(e => e.Nickname)
                      .HasColumnType("varchar(50)")
                      .IsRequired(false);

                entity.Property(e => e.Introduction)
                      .HasColumnType("text")
                      .IsRequired(false);
            });

            // 관계 설정
            modelBuilder.Entity<User>()
                .HasOne(u => u.Stats)
                .WithOne(s => s.User)
                .HasForeignKey<UserStats>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Resources)
                .WithOne(r => r.User)
                .HasForeignKey<UserResources>(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Profile)
                .WithOne(p => p.User)
                .HasForeignKey<UserProfiles>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserStats> UserStats { get; set; }
        public DbSet<UserResources> UserResources { get; set; }
        public DbSet<UserProfiles> UserProfiles { get; set; }
    }
}
