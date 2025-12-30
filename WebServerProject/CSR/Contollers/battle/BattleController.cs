using Microsoft.AspNetCore.Mvc;
using WebServerProject.CSR.Services;
using WebServerProject.Models.Request.Battle;
using WebServerProject.Models.Response.Battle;

namespace WebServerProject.CSR.Contollers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class BattleController : ControllerBase
    {
        private readonly ILogger<BattleController> _logger;
        private readonly IBattleService _battleService;

        public BattleController(
            ILogger<BattleController> logger,
            IBattleService battleService)
        {
            _logger = logger;
            _battleService = battleService;
        }

        [HttpPost("stage/start")]
        public async Task<StartStageBattleResponse> StartStageBattleAsync([FromBody] StartStageBattleRequest reqeust)
        {
            try
            {
                var result = await _battleService.StartStageBattleAsync(reqeust.userId, reqeust.stageId, reqeust.deckIndex);

                return new StartStageBattleResponse
                {
                    success = result.success,
                    message = result.message,
                    rewardGold = result.rewardGold,
                    rewardExp = result.rewardExp
                    // TODO : 결과
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "배틀 요청 중 서버 예외 발생");
                return new StartStageBattleResponse
                {
                    success = false,
                    message = "배틀 요청 중 오류가 발생했습니다. 관리자에게 문의하세요."
                };
            }
        }
    }
}
