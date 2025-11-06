using SqlKata.Execution;
using WebServerProject.Models.Entities.User;
using WebServerProject.Repositories;

namespace WebServerProject.Services
{
    public interface ICharacterService
    {
        public Task<List<UserCharacter>?> GetUserCharacterList(int userId);
    }

    public class CharacterService : ICharacterService
    {
        public readonly ICharacterRepository _characterRepository;

        public CharacterService(ICharacterRepository characterRepository)
        {
            _characterRepository = characterRepository;
        }

        public async Task<List<UserCharacter>?> GetUserCharacterList(int userId)
        {
            var userCharacter = await _characterRepository.GetUserCharacterListAsync(userId);
            if(userCharacter.Any())
            {
                return userCharacter;
            }
            else
            {
                return null;
            }
        }
    }
}
