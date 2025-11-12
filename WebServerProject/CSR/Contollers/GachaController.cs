using Microsoft.AspNetCore.Mvc;
using WebServerProject.CSR.Services;
using WebServerProject.Models.Request;
using WebServerProject.Models.Response;

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
            var gachaList = await _gachaService.GetGachaListAsync();
            if(gachaList == null)
            {
                return new GachaListResponse
                {
                    success = false,
                    message = "가챠 목록을 불러오는 데 실패했습니다."
                };
            }
            else if(gachaList.Count == 0)
            {
                return new GachaListResponse
                {
                    success = false,
                    message = "가챠 목록이 없습니다."
                };
            }

            return new GachaListResponse
            {
                success = true,
                message = "가챠 목록을 불러왔습니다.",
                gachaList = gachaList
            };
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
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "가챠 처리 중 비즈니스 예외 발생: {Message}", ex.Message);
                return new GachaDrawResponse{
                    success = false,
                    message = ex.Message
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "가챠 뽑기 중 예외 발생: {Message}", ex.Message);
                return new GachaDrawResponse
                {
                    success = false,
                    message = "가챠 뽑기 중 오류가 발생했습니다. " + ex.Message
                };
            }
        }
    }
}
