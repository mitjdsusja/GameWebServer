using Microsoft.AspNetCore.Mvc;
using WebServerProject.CSR.Services.Character;
using WebServerProject.Models.Request.Character;
using WebServerProject.Models.Response.Character;

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
            try
            {
                var characterList = await _characterService.GetUserCharacterDetailList(request.userId);
                
                return new CharacterListResponse
                {
                    success = true,
                    characters = characterList
                };
            }
            catch (InvalidOperationException ex)
            {
                return new CharacterListResponse
                {
                    success = false,
                    message = ex.Message,
                };
            }
            catch (Exception ex)
            {
                return new CharacterListResponse
                {
                    success = false,
                    message = "캐릭터 목록을 불러오는 중 오류가 발생했습니다."
                };
            }
            

            
        }
    }
}
