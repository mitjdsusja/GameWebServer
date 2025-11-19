using WebServerProject.CSR.Repositories.Enemy;
using WebServerProject.CSR.Repositories.Stage;
using WebServerProject.Models.DTOs.Enemy;
using WebServerProject.Models.DTOs.Stage;

namespace WebServerProject.CSR.Services.Stage
{
    public interface IStageService
    {
        public Task<StageDTO> GetStageAsync(int stageId);
    }

    public class StageService : IStageService
    {
        private readonly IStageRepository _stageRepository;
        private readonly IEnemyRepository _enemyRepository;

        public StageService(
            IStageRepository stageRepository,
            IEnemyRepository enemyRepository)
        {
            _stageRepository = stageRepository;
            _enemyRepository = enemyRepository;
        }   

        public async Task<StageDTO> GetStageAsync(int stageId)
        {
            StageDTO stageDTO;

            // 스테이지 정보 조회
            var stage = await _stageRepository.GetStageAsync(stageId);
            if(stage == null)
            {
                throw new InvalidOperationException("스테이지 정보를 찾을 수 없습니다.");
            }
            stageDTO = StageDTO.FromStage(stage);


            // 적 정보 조회
            var stageEnemies = await _stageRepository.GetStageEnemyListAsync(stageId);
            if(stageEnemies == null)
            {
                throw new InvalidOperationException("스테이지 적 정보를 찾을 수 없습니다.");
            }

            // 적 Teplate 조회
            List<StageEnemyDTO> enemyDTOs = new List<StageEnemyDTO>();
            foreach (var e in stageEnemies)
            {
                StageEnemyDTO stageEnemy = StageEnemyDTO.FromStageEnemy(e);

                var enemyTemplate = await _enemyRepository.GetEnemyTemplateAsync(e.enemy_id);
                if(enemyTemplate == null)
                {
                    throw new InvalidOperationException("적 템플릿 정보를 찾을 수 없습니다.");
                }

                EnemyTemplateDTO enemyTemplateDTO = EnemyTemplateDTO.FromEnemyTemplate(enemyTemplate);

                stageEnemy.enemyTemplate = enemyTemplateDTO;

                enemyDTOs.Add(stageEnemy);
            }

            stageDTO.stageEnemies = enemyDTOs;

            return stageDTO;
        }
    }
}
