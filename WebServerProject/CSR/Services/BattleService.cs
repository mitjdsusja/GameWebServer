using WebServerProject.Models.DTOs.Battle;

namespace WebServerProject.CSR.Services
{
    public interface IBattleService
    {
        public Task<StartStageBattleResult> StartStageBattleAsync(int userId, int stageId, int deckId);
    }

    public class BattleService : IBattleService
    {
        private readonly IUserService _userService;
        private readonly IStageService _stageService;
        private readonly IDeckService _deckService;

        public BattleService(
            IUserService userService,
            IStageService stageService,
            IDeckService deckService
            )
        {
            _userService = userService;
            _stageService = stageService;
            _deckService = deckService;
        }
        public async Task<StartStageBattleResult> StartStageBattleAsync(int userId, int stageId, int deckId)
        {
            // 유저 검증 
            var userDTO = await _userService.GetUserAsync(userId);

            // 스테이지 검증
            var stageDTO = await _stageService.GetStageAsync(stageId);

            // 덱 검증
            var deckDTO = await _deckService.GetDeckAsync(deckId);

            // TODO : 전투

            return new StartStageBattleResult
            {
                success = true,
                message = "전투가 성공적으로 시작되었습니다.",
                rewardGold = stageDTO.rewardGold,
                rewardExp = stageDTO.rewardExp
            };
        }
    }
}
