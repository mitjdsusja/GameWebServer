using WebServerProject.Models.Entities.User;

namespace WebServerProject.Models.Entities.Master
{
    public class CharacterTemplate
    {
        public int id { get; set; }
        public string name { get; set; } = null!;
        public string? description { get; set; }
        public string rarity { get; set; } = "Common";
        public int base_hp { get; set; }
        public int base_attack { get; set; }
        public int Base_defense { get; set; }
    }
}
