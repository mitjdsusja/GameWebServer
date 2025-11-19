using WebServerProject.CSR.Repositories.Character;
using WebServerProject.Models.DTOs.Character;

namespace WebServerProject.CSR.Services.Character
{
    public interface ICharacterService
    {
        public Task<List<CharacterDetailDTO>?> GetUserCharacterDetailList(int userId);
    }

    public class CharacterService : ICharacterService
    {
        public readonly ICharacterRepository _characterRepository;

        public CharacterService(ICharacterRepository characterRepository)
        {
            _characterRepository = characterRepository;
        }

        public async Task<List<CharacterDetailDTO>?> GetUserCharacterDetailList(int userId)
        {
            var result = new List<CharacterDetailDTO>();

            // 유저 캐릭터 목록 조회
            var characters = await _characterRepository.GetUserCharacterListAsync(userId);
            if (characters == null || characters.Count == 0)
                return result;

            // 해당 캐릭터들이 사용하는 템플릿 ID만 추출
            var templateIds = characters.Select(c => c.template_id).Distinct().ToList();

            // 템플릿 일괄 조회
            var templates = await _characterRepository.GetCharacterTemplateListAsync(templateIds);
            var templateMap = templates.ToDictionary(t => t.id, t => t);

            // 캐릭터 + 템플릿 조합 후 DTO 변환
            foreach (var c in characters)
            {
                if (templateMap.TryGetValue(c.template_id, out var tmpl))
                {
                    var dto = new CharacterDetailDTO
                    {
                        userCharacter = UserCharacterDTO.FromUserCharacter(c),
                        characterTemplate = CharacterTemplateDTO.FromCharacterTemplate(tmpl)
                    };
                    result.Add(dto);
                }
            }

            return result;
        }
    }
}
