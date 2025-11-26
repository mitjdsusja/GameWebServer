using WebServerProject.Models.Entities.CharacterEntity;

namespace WebServerProject.Models.DTOs.Character
{
    public class CharacterTemplateDTO
    {
        public string name { get; set; }
        public string? description { get; set; }
        public int rarity { get; set; }
        public int baseHp { get; set; }
        public int baseAttack { get; set; }
        public int BaseDefense { get; set; }

        public static CharacterTemplateDTO FromCharacterTemplate(CharacterTemplate characterTemplate)
        {
            var model = new CharacterTemplateDTO
            {
                name = characterTemplate.name,
                description = characterTemplate.description,
                rarity = characterTemplate.rarity,
                baseHp = characterTemplate.base_hp,
                baseAttack = characterTemplate.base_attack,
                BaseDefense = characterTemplate.Base_defense
            };
            return model;
        }
    }
}
