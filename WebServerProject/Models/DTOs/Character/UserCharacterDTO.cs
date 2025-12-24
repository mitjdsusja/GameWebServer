using WebServerProject.Models.Entities.CharacterEntity;

namespace WebServerProject.Models.DTOs.Character
{
    public class UserCharacterDTO
    {
        public int userId { get; set; }
        public int userCharacterId { get; set; }
        public int templateId { get; set; }
        public int level { get; set; } = 1;
        public int experience { get; set; }
        public int stars { get; set; }
        public DateTime obtainedAt { get; set; }

        public static UserCharacterDTO FromUserCharacter(UserCharacter character)
        {
            var result = new UserCharacterDTO
            {
                userId = character.user_id,
                userCharacterId = character.id,
                templateId = character.template_id,
                level = character.level,
                experience = character.experience,
                stars = character.stars,
                obtainedAt = character.obtainedAt
            };
            return result;
        }
    }
}
