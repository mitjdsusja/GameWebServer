using WebServerProject.CSR.Services.Deck;
using WebServerProject.CSR.Services.Stage;
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
            // 임시로 총 공격력 비교 후 승패 결정
            int totalAttackPower = 0;
            foreach (var slot in deckDTO.deckSlots)
            {
                totalAttackPower += slot.characterDetailDTO.characterTemplate.base_attack;
            }
            int totalEnemyAttackPower = 0;
            foreach (var enemy in stageDTO.stageEnemies)
            {
                totalEnemyAttackPower += enemy.enemyTemplate.attack;
            }

            // 전투 결과 반환
            if (totalAttackPower <= totalEnemyAttackPower)
            {
                return new StartStageBattleResult
                {
                    success = false,
                    message = "전투에서 패배하였습니다.",
                    rewardGold = 0,
                    rewardExp = 0
                };
            }
            else
            {
                return new StartStageBattleResult
                {
                    success = true,
                    message = "전투에서 승리하였습니다.",
                    rewardGold = stageDTO.rewardGold,
                    rewardExp = stageDTO.rewardExp
                };
            }
        }
    }
}
