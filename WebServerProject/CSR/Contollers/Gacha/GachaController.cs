using Microsoft.AspNetCore.Mvc;
using WebServerProject.CSR.Services;
using WebServerProject.Models.Request;
using WebServerProject.Models.Request.gacha;
using WebServerProject.Models.Response;
using WebServerProject.Models.Response.gacha;

namespace WebServerProject.CSR.Contollers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GachaController : ControllerBase
    {
        private readonly ILogger<GachaController> _logger;

        private readonly IGachaService _gachaService;

        public GachaController(
            ILogger<GachaController> logger,
            IGachaService gachaService )
        {
            _logger = logger;
            _gachaService = gachaService;
        }

        [HttpPost("list")]
        public async Task<GachaListResponse> GetList([FromBody] GachaListRequest req)
        {
            try
            {
                var gachaList = await _gachaService.GetGachaListAsync();

                return new GachaListResponse
                {
                    success = true,
                    message = "가챠 목록을 불러왔습니다.",
                    gachaList = gachaList
                };
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "가챠 목록 조회 중 예외 발생: {Message}", ex.Message);

                return new GachaListResponse
                {
                    success = false,
                    message = ex.Message
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "가챠 목록 조회 중 서버 예외 발생");

                return new GachaListResponse
                {
                    success = false,
                    message = "가챠 목록을 불러오는 중 오류가 발생했습니다. 관리자에게 문의하세요."
                };
            }
        }

        [HttpPost("draw")]
        public async Task<GachaDrawResponse> Draw([FromBody] GachaDrawRequest req)
        {
            try
            {
                var result = await _gachaService.DrawAsync(req.gachaCode, req.userId);

                return new GachaDrawResponse
                {
                    success = result.Success,
                    message = result.Message,
                    drawnItem = result.DrawnItem,
                    remainingResources = result.RemainingResources
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "가챠 뽑기 중 예외 발생: {Message}", ex.Message);
                return new GachaDrawResponse
                {
                    success = false,
                    message = "가챠 뽑기 중 오류가 발생했습니다. "
                };
            }
        }
    }
}
