using Microsoft.EntityFrameworkCore;
using WebServerProject.Models.Entities.Master;
using WebServerProject.Models.Entities.User;

namespace WebServerProject.Data
{
    public class CharacterDbContext : DbContext
    {
        public CharacterDbContext(DbContextOptions<CharacterDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CharacterTemplate>(entity =>
            {
                entity.ToTable("Character_Template");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name).HasColumnType("varchar(50)").IsRequired();
                entity.Property(e => e.Description).HasColumnType("text").IsRequired(false); ;
                entity.Property(e => e.Rarity).HasColumnType("integer").IsRequired();
                entity.Property(e => e.BaseHp).HasColumnType("integer").IsRequired();
                entity.Property(e => e.BaseAttack).HasColumnType("integer").IsRequired();
                entity.Property(e => e.BaseDefense).HasColumnType("integer").IsRequired();
            });

            modelBuilder.Entity<UserCharacter>(entity =>
            {
                entity.ToTable("User_Character");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.AccountId).HasColumnType("integer").IsRequired();
                entity.Property(e => e.TemplateId).HasColumnType("integer").IsRequired();
                entity.Property(e => e.Level).HasColumnType("integer").HasDefaultValue(1).IsRequired();
                entity.Property(e => e.Experience).HasColumnType("integer").HasDefaultValue(0).IsRequired();
                entity.Property(e => e.Stars).HasColumnType("integer").IsRequired();
                entity.Property(e => e.ObtainedAt).HasColumnType("datetime").IsRequired();

                entity.HasOne(e => e.Template)
                      .WithMany(t => t.UserCharacters)
                      .HasForeignKey(e => e.TemplateId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
