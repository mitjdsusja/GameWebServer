using SqlKata.Execution;
using System.Data;
using WebServerProject.CSR.Services.Character;
using WebServerProject.Models.Entities.CharacterEntity;

namespace WebServerProject.CSR.Repositories.Character
{
    public interface ICharacterRepository
    {
        public Task<UserCharacter> GetUserCharacterAsync(int userId, int characterTemplateId, QueryFactory? db = null, IDbTransaction? tx = null);
        public Task<List<UserCharacter>> GetUserCharacterListAsync(int userId);
        public Task<List<UserCharacter>> GetUserCharacterListAsync(List<int> userCharacterIds);
        public Task<List<CharacterTemplate>> GetCharacterTemplateListAsync(List<int> templateIds);
        public Task<UserCharacter> GetUserCharacterAsync(int userCharacterId, QueryFactory? db = null, IDbTransaction? tx = null);
        public Task<CharacterTemplate> GetCharacterTemplateAsync(int templateId);
        public Task<int> AddCharacterToUserAsync(int userId, int characterId, QueryFactory? db = null, IDbTransaction? tx = null);
    }

    public class CharacterRepository : ICharacterRepository
    {
        private readonly QueryFactory _db;

        public CharacterRepository(QueryFactory db)
        {
            _db = db;
        }
        public async Task<UserCharacter> GetUserCharacterAsync(int userId, int userCharacterId, QueryFactory? db = null, IDbTransaction? tx = null)
        {
            var q = db ?? _db;

            var userCharacter = await q.Query("user_characters")
                                             .Where("id", userCharacterId)
                                             .Where("user_id", userId)
                                             .FirstOrDefaultAsync<UserCharacter>(tx);

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

        public async Task<List<UserCharacter>> GetUserCharacterListAsync(List<int> userCharacerIds)
        {
            var characters = await _db.Query("user_characters")
                                      .WhereIn("id", userCharacerIds)
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

        public async Task<int> AddCharacterToUserAsync(int userId, int characterId, QueryFactory? db = null, IDbTransaction? tx = null)
        {
            var q = db ?? _db;

            var result = await q.Query("user_characters")
                              .InsertAsync(new
                              {
                                  user_id = userId,
                                  template_id = characterId,
                                  level = 1,
                                  experience = 1,
                                  stars = 1,
                                  obtained_at = DateTime.UtcNow
                              }, tx);

            return result;
        }

        public async Task<UserCharacter> GetUserCharacterAsync(int userCharacterId, QueryFactory? db = null, IDbTransaction? tx = null)
        {
            var q = db ?? _db;

            var result = await q.Query("user_characters")
                                  .Where("id", userCharacterId)
                                  .FirstOrDefaultAsync<UserCharacter>(tx);
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
