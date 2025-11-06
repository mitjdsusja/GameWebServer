using SqlKata.Execution;
using WebServerProject.Models.Entities.User;

namespace WebServerProject.Repositories
{
    public interface ICharacterRepository
    {
        public Task<List<UserCharacter>> GetUserCharacterListAsync(int userId);
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
    }
}
