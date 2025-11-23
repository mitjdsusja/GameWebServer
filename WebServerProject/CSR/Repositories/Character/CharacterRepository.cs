using SqlKata.Execution;
using WebServerProject.CSR.Services.Character;
using WebServerProject.Models.Entities.CharacterEntity;

namespace WebServerProject.CSR.Repositories.Character
{
    public interface ICharacterRepository
    {
        public Task<List<UserCharacter>> GetUserCharacterListAsync(int userId);
        public Task<List<CharacterTemplate>> GetCharacterTemplateListAsync(List<int> templateIds);
        public Task<UserCharacter> GetUserCharacterAsync(int userCharacterId);
        public Task<CharacterTemplate> GetCharacterTemplateAsync(int templateId);
        public Task<AddCharacterResultDTO> AddCharacterToUserAsync(int userId, int characterId);
    }

    public class CharacterRepository : ICharacterRepository
    {
        private readonly QueryFactory _db;

        public CharacterRepository(QueryFactory db)
        {
            _db = db;
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

        public async Task<AddCharacterResultDTO> AddCharacterToUserAsync(int userId, int characterId)
        {
            // 중복 확인
            var existingCharacter = await _db.Query("user_characters")
                                             .Where("user_id", userId)
                                             .Where("template_id", characterId)
                                             .FirstOrDefaultAsync<UserCharacter>();
            if (existingCharacter != null)
            {
                return new AddCharacterResultDTO
                {
                    Success = false,
                    Message = "이미 보유한 캐릭터입니다.",
                    isNew = false
                };
            }

            var result = await _db.Query("user_characters")
                              .InsertAsync(new
                              {
                                  account_id = userId,
                                  template_id = characterId,
                                  level = 1,
                                  experience = 1,
                                  stars = 1,
                                  obtained_at = DateTime.UtcNow
                              });

            return new AddCharacterResultDTO
            {
                Success = result > 0,
                Message = result > 0 ? "캐릭터가 성공적으로 추가되었습니다." : "캐릭터 추가에 실패했습니다.",
                isNew = result > 0
            };
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
