using Microsoft.EntityFrameworkCore;
using WebServerProject.Models.Entities;

namespace WebServerProject.Data
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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
        }

        public DbSet<User> Users { get; set; }
    }
}
