using MySqlConnector;
using SqlKata.Compilers;
using SqlKata.Execution;
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
        private readonly string _connectionString;
        private readonly IUserService _userService;
        private readonly IStageService _stageService;
        private readonly IDeckService _deckService;

        public BattleService(
            IConfiguration config,
            IUserService userService,
            IStageService stageService,
            IDeckService deckService
            )
        {
            _connectionString = config.GetConnectionString("GameDb")
            ?? throw new InvalidOperationException("ConnectionStrings:GameDb is missing.");
            _userService = userService;
            _stageService = stageService;
            _deckService = deckService;
        }
        public async Task<StartStageBattleResult> StartStageBattleAsync(int userId, int stageId, int deckIndex)
        {
            // 스테이지 정보
            var stageDTO = await _stageService.GetStageAsync(stageId);

            // 덱 정보
            var deckDTO = await _deckService.GetDeckAsync(userId, deckIndex);

            // TODO : 전투
            // 임시로 총 공격력 비교 후 승패 결정
            int totalAttackPower = 0;
            if(deckDTO.deckSlots == null)
            {
                throw new InvalidOperationException($"User{userId} 덱{deckIndex} {deckDTO.deckIndex}의 슬롯이 없습니다.");
            }

            foreach (var slot in deckDTO.deckSlots)
            {
                if(slot.characterDetail == null)
                {
                    continue;
                }
                totalAttackPower += slot.characterDetail.characterTemplate.baseAttack;
            }
            int totalEnemyAttackPower = 0;
            foreach (var enemy in stageDTO.stageEnemies)
            {
                totalEnemyAttackPower += enemy.enemyTemplate.attack;
            }

            bool isWin = totalAttackPower >= totalEnemyAttackPower;

            // 패배면 DB 작업 없이 바로 반환
            if (!isWin)
            {
                return new StartStageBattleResult
                {
                    success = false,
                    message = "전투에서 패배하였습니다.",
                    rewardGold = 0,
                    rewardExp = 0
                };
            }

            // 결과 저장(보상 지급)
            await using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();
            var db = new QueryFactory(conn, new MySqlCompiler());
            await using var tx = await conn.BeginTransactionAsync();

            BattleRewardDTO battleReward = new BattleRewardDTO();
            battleReward.userId = userId;   
            battleReward.gold = stageDTO.rewardGold;
            battleReward.exp = stageDTO.rewardExp;

            try
            {
                await _userService.GrantBattleRewardAsync(battleReward, db, tx);

                await tx.CommitAsync();

                return new StartStageBattleResult
                {
                    success = true,
                    message = "전투에서 승리하였습니다.",
                    rewardGold = battleReward.gold,
                    rewardExp = battleReward.exp
                };
            }
            catch
            {
                try { await tx.RollbackAsync(); } catch { }
                throw;
            }   
        }
    }
}
