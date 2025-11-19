using Microsoft.AspNetCore.Mvc;
using WebServerProject.CSR.Services;
using WebServerProject.Models.Response;
using WebServerProject.Models.Request;
using WebServerProject.Models.Response.Deck;
using WebServerProject.Models.Request.Deck;
using WebServerProject.CSR.Services.Deck;

namespace WebServerProject.CSR.Contollers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeckController : ControllerBase
    {
        private readonly ILogger<DeckController> _logger;
        private readonly IDeckService _deckService;

        public DeckController (
            ILogger<DeckController> logger,
            IDeckService deckService)
        {
            _logger = logger;
            _deckService = deckService;
        }

        [HttpPost("list")]
        public async Task<DeckListResponse> GetDeckListAsync([FromBody] DeckListRequest req)
        {
            try
            {
                var decks = await _deckService.GetDeckListAsync(req.userId);

                return new DeckListResponse
                {
                    success = true,
                    message = "덱 목록을 불러왔습니다.",
                    decks = decks
                };
            }
            catch(InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "덱 목록 조회 중 예외 발생: {Message}", ex.Message);
                return new DeckListResponse
                {
                    success = false,
                    message = ex.Message
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "덱 목록 조회 중 서버 예외 발생");
                return new DeckListResponse
                {
                    success = false,
                    message = "덱 목록을 불러오는 중 오류가 발생했습니다. 관리자에게 문의하세요."
                };
            }
        }

        [HttpPost("update")]
        public async Task<DeckUpdateResponse> UpdateDeckAsync([FromBody] DeckUpdateRequest req)
        {
            try
            {
                var updatedDeck = await _deckService.UpdateDeckAsync(req.userId, req.deckIndex, req.characterIds);

                return new DeckUpdateResponse
                {
                    success = true,
                    message = "덱이 성공적으로 업데이트되었습니다.",
                    updatedDeck = updatedDeck
                };
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "덱 업데이트 중 예외 발생: {Message}", ex.Message);
                return new DeckUpdateResponse
                {
                    success = false,
                    message = ex.Message
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "덱 업데이트 중 서버 예외 발생");
                return new DeckUpdateResponse
                {
                    success = false,
                    message = "덱을 업데이트하는 중 오류가 발생했습니다. 관리자에게 문의하세요."
                };
            }
        }
    }
}
