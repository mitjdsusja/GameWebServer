using SqlKata.Execution;
using WebServerProject.CSR.Repositories;
using WebServerProject.Models.Entities.User;

namespace WebServerProject.CSR.Services
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
