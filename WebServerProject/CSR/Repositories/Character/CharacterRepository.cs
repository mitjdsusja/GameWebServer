using SqlKata.Execution;
using WebServerProject.CSR.Services.Character;
using WebServerProject.Models.Entities.CharacterEntity;

namespace WebServerProject.CSR.Repositories.Character
{
    public interface ICharacterRepository
    {
        public Task<UserCharacter> GetUserCharacterAsync(int userId, int characterTemplateId);
        public Task<List<UserCharacter>> GetUserCharacterListAsync(int userId);
        public Task<List<CharacterTemplate>> GetCharacterTemplateListAsync(List<int> templateIds);
        public Task<UserCharacter> GetUserCharacterAsync(int userCharacterId);
        public Task<CharacterTemplate> GetCharacterTemplateAsync(int templateId);
        public Task<int> AddCharacterToUserAsync(int userId, int characterId);
    }

    public class CharacterRepository : ICharacterRepository
    {
        private readonly QueryFactory _db;

        public CharacterRepository(QueryFactory db)
        {
            _db = db;
        }
        public async Task<UserCharacter> GetUserCharacterAsync(int userId, int userCharacterId)
        {
            var userCharacter = await _db.Query("user_characters")
                                             .Where("id", userCharacterId)
                                             .Where("user_id", userId)
                                             .FirstOrDefaultAsync<UserCharacter>();

            return userCharacter;
        }

        public async Task<List<UserCharacter>> GetUserCharacterListAsync(int userId)
        {
            // 유저 캐릭터 목록 조회
            var characters = await _db.Query("user_characters")
                                      .Where("user_id", userId)
                                      .GetAsync<UserCharacter>();

            return characters.ToList();
        }

        public async Task<List<CharacterTemplate>> GetCharacterTemplateListAsync(List<int> templateIds)
        {
            // 캐릭터 템플릿 목록 조회
            var templates = await _db.Query("character_templates")
                                     .WhereIn("id", templateIds)
                                     .GetAsync<CharacterTemplate>();
            return templates.ToList();
        }

        public async Task<int> AddCharacterToUserAsync(int userId, int characterId)
        {
            var result = await _db.Query("user_characters")
                              .InsertAsync(new
                              {
                                  user_id = userId,
                                  template_id = characterId,
                                  level = 1,
                                  experience = 1,
                                  stars = 1,
                                  obtained_at = DateTime.UtcNow
                              });

            return result;
        }

        public async Task<UserCharacter> GetUserCharacterAsync(int userCharacterId)
        {
            var result = await _db.Query("user_characters")
                                  .Where("id", userCharacterId)
                                  .FirstOrDefaultAsync<UserCharacter>();
            return result;  
        }

        public async Task<CharacterTemplate> GetCharacterTemplateAsync(int templateId)
        {
            var result = await _db.Query("character_templates")
                                  .Where("id", templateId)
                                  .FirstOrDefaultAsync<CharacterTemplate>();
            return result;
        }
    }
}
