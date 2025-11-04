using WebServerProject.Models.Entities.Master;

namespace WebServerProject.Models.Entities.User
{
    public class UserCharacter
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public int TemplateId { get; set; }
        public int Level { get; set; } = 1;
        public int Experience { get; set; } = 0;
        public int Stars { get; set; } = 1;
        public DateTime ObtainedAt { get; set; } = DateTime.UtcNow;

        // 관계
        public User Account { get; set; } = null!;
        public CharacterTemplate Template { get; set; } = null!;
    }
}
