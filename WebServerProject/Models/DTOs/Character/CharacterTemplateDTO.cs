using WebServerProject.Models.Entities.CharacterEntity;

namespace WebServerProject.Models.DTOs.Character
{
    public class CharacterTemplateDTO
    {
        public string name { get; set; }
        public string? description { get; set; }
        public int rarity { get; set; }
        public int base_hp { get; set; }
        public int base_attack { get; set; }
        public int Base_defense { get; set; }

        public static CharacterTemplateDTO FromCharacterTemplate(CharacterTemplate characterTemplate)
        {
            var model = new CharacterTemplateDTO
            {
                name = characterTemplate.name,
                description = characterTemplate.description,
                rarity = characterTemplate.rarity,
                base_hp = characterTemplate.base_hp,
                base_attack = characterTemplate.base_attack,
                Base_defense = characterTemplate.Base_defense
            };
            return model;
        }
    }
}
