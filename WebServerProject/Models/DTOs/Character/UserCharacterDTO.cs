using WebServerProject.Models.Entities.Character;

namespace WebServerProject.Models.DTOs.Character
{
    public class UserCharacterDTO
    {
        public int account_id { get; set; }
        public int template_id { get; set; }
        public int level { get; set; } = 1;
        public int experience { get; set; }
        public int stars { get; set; }
        public DateTime obtainedAt { get; set; }

        public static UserCharacterDTO FromUserCharacter(UserCharacter character)
        {
            var result = new UserCharacterDTO
            {
                account_id = character.account_id,
                template_id = character.template_id,
                level = character.level,
                experience = character.experience,
                stars = character.stars,
                obtainedAt = character.obtainedAt
            };
            return result;
        }
    }
}
