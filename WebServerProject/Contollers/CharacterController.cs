using Microsoft.AspNetCore.Mvc;
using WebServerProject.Models.DTOs.Request;
using WebServerProject.Models.DTOs.Response;
using WebServerProject.Services;

namespace WebServerProject.Contollers
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
            var characters = await _characterService.GetUserCharacterList(request.userId);
            if (characters == null)
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
                characters = characters
            };
        }
    }
}
