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
        private readonly IGachaService _gachaService;

        public GachaController(IGachaService gachaService )
        {
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
            var result = await _gachaService.DrawAsync(req.gachaCode, req.userId);

            if(result == null)
            {
                return new GachaDrawResponse
                {
                    success = false,
                    message = "가챠 뽑기에 실패했습니다."
                };
            }

            return new GachaDrawResponse
            {
                success = result.Success,
                message = result.Message,
                drawnItem = result.DrawnItem,
                remainingResources = result.RemainingResources
            };
        }
    }
}
