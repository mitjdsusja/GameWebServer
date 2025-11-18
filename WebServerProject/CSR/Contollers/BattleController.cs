using Microsoft.AspNetCore.Mvc;
using WebServerProject.CSR.Services;
using WebServerProject.Models.Request;
using WebServerProject.Models.Response;

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
                var result = await _battleService.StartStageBattleAsync(reqeust.userId, reqeust.stageId, reqeust.deckId);

                return new StartStageBattleResponse
                {
                    success = result.success,
                    message = result.message
                    // TODO : 결과
                };
            }
            catch(InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation while starting stage battle");
                return new StartStageBattleResponse
                {
                    success = false,
                    message = ex.Message
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting stage battle"); 
                return new StartStageBattleResponse
                {
                    success = false,
                    message = ex.Message
                };
            }
        }
    }
}
