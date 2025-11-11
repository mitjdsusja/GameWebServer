using Microsoft.AspNetCore.Mvc;
using WebServerProject.CSR.Services;
using WebServerProject.Models.DTOs.Character;
using WebServerProject.Models.DTOs.User;
using WebServerProject.Models.Entities.User;
using WebServerProject.Models.Request;
using WebServerProject.Models.Response;

namespace WebServerProject.CSR.Contollers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CharacterController : ControllerBase
    {
        private readonly ICharacterService _characterService;

        public CharacterController(ICharacterService characterService)
        {
            _characterService = characterService;
        }

        [HttpPost("list")]
        public async Task<CharacterListResponse> GetCharacterList([FromBody] CharacterListRequest request)
        {
            var characterList = await _characterService.GetUserCharacterDetailList(request.userId);
            if (characterList == null || characterList.Count == 0)
            {
                return new CharacterListResponse
                {
                    success = false,
                    message = "캐릭터 없음"
                };
            }

            return new CharacterListResponse
            {
                success = true,
                characters = characterList
            };
        }
    }
}
