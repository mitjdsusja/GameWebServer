using WebServerProject.Models.Entities.User;

namespace WebServerProject.Models.Entities.Master
{
    public class CharacterTemplate
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string Rarity { get; set; } = "Common";
        public int BaseHp { get; set; }
        public int BaseAttack { get; set; }
        public int BaseDefense { get; set; }

        public ICollection<UserCharacter> UserCharacters { get; set; } = new List<UserCharacter>();
    }
}
