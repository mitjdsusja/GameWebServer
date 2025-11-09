using SqlKata.Execution;
using WebServerProject.Models.Character;
using WebServerProject.Models.Entities.User;

namespace WebServerProject.Repositories
{
    public interface ICharacterRepository
    {
        public Task<List<UserCharacter>> GetUserCharacterListAsync(int userId);
        public Task<AddCharacterResult> AddCharacterToUser(int userId, int characterId);
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
            var result = await _db.Query("user_characters")
                                  .Where("account_id", userId)
                                  .GetAsync<UserCharacter>();

            return result.ToList();
        }

        public async Task<AddCharacterResult> AddCharacterToUser(int userId, int characterId)
        {
            // 중복 확인
            var existingCharacter = await _db.Query("user_characters")
                                             .Where("account_id", userId)
                                             .Where("template_id", characterId)
                                             .FirstOrDefaultAsync<UserCharacter>();
            if (existingCharacter != null)
            {
                return new AddCharacterResult
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

            return new AddCharacterResult
            {
                Success = result > 0,
                Message = result > 0 ? "캐릭터가 성공적으로 추가되었습니다." : "캐릭터 추가에 실패했습니다.",
                isNew = result > 0
            };
        }
    }
}
