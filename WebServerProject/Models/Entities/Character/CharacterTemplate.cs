

namespace WebServerProject.Models.Entities.CharacterEntity
{
    public class CharacterTemplate
    {
        public int id { get; set; }
        public string name { get; set; } = null!;
        public string? description { get; set; }
        public int rarity { get; set; }
        public int base_hp { get; set; }
        public int base_attack { get; set; }
        public int Base_defense { get; set; }
    }
}
