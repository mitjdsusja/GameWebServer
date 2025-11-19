using Microsoft.AspNetCore.Mvc;
using WebServerProject.CSR.Services.Stage;
using WebServerProject.Models.Request.Stage;
using WebServerProject.Models.Response.Stage;

namespace WebServerProject.CSR.Contollers.Stage
{
    [ApiController]
    [Route("api/[controller]")]
    public class StageController : ControllerBase
    {
        private readonly ILogger<StageController> _logger;
        private readonly IStageService _stageService;

        public StageController(
            ILogger<StageController> logger,
            IStageService stageService)
        {
            _logger = logger;
            _stageService = stageService;
        }

        [HttpPost("list")]
        public async Task<StageListResponse> GetStageListAsync([FromBody]StageListRequest request)
        {
            try
            {
                var stages = await _stageService.GetStageListAsync(request.chapterId);

                return new StageListResponse
                {
                    success = true,
                    message = "스테이지 리스트를 불러왔습니다.",
                    stages = stages
                };
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "스테이지 리스트를 불러오는중 오류가 발생했습니다. ");
                return new StageListResponse
                {
                    success = false,
                    message = ex.Message,
                };
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "스테이지 리스트를 불러오는중 시스템 오류가 발생했습니다.");
                return new StageListResponse
                {
                    success = false,
                    message = "스테이지 리스트를 불러오는중 오류가 발생했습니다.",
                };
            }
        }
    }
}
