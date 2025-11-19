using SqlKata.Execution;

using StageEntity = WebServerProject.Models.Entities.StageEntity.Stage;
using StageEnemyEntity = WebServerProject.Models.Entities.StageEntity.StageEnemy;

namespace WebServerProject.CSR.Repositories.Stage
{
    public interface IStageRepository
    {
        public Task<List<StageEntity>> GetStageListAsync();
        public Task<List<StageEntity>> GetStageListAsync(int chapterId);
        public Task<StageEntity> GetStageAsync(int stageId);
        public Task<StageEntity> GetStageAsync(int chapterId, int stageNumber);
        public Task<List<StageEnemyEntity>> GetStageEnemyListAsync(int stageId);
    }

    public class StageRepository : IStageRepository
    {
        private readonly QueryFactory _db;

        public StageRepository(QueryFactory db)
        {
            _db = db;
        }

        public async Task<List<StageEntity>> GetStageListAsync()
        {
            var stages = await _db.Query("stages")
                                  .GetAsync<StageEntity>();

            return stages.ToList();
        }

        public async Task<List<StageEntity>> GetStageListAsync(int chapterId)
        {
            var stages = await _db.Query("stages")
                                  .Where("chapter", chapterId)
                                  .OrderBy("stage_number")
                                  .GetAsync<StageEntity>();

            return stages.ToList();
        }

        public async Task<StageEntity> GetStageAsync(int stageId)
        {
            var stage = await _db.Query("stages")
                                 .Where("id", stageId)
                                 .FirstOrDefaultAsync<StageEntity>();

            return stage;
        }

        public async Task<StageEntity> GetStageAsync(int chapterId, int stageNumber)
        {
            var stage = await _db.Query("stages")
                                 .Where("chapter", chapterId)
                                 .Where("stage_number", stageNumber)
                                 .FirstOrDefaultAsync<StageEntity>();
            return stage;
        }

        public async Task<List<StageEnemyEntity>> GetStageEnemyListAsync(int stageId)
        {
            var stageEnemies = await _db.Query("stage_enemies")
                                        .Where("stage_id", stageId)
                                        .GetAsync<StageEnemyEntity>();
            return stageEnemies.ToList();
        }
    }
}
