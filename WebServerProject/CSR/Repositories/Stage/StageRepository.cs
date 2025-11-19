using SqlKata.Execution;
using WebServerProject.Models.Entities.Stage;

namespace WebServerProject.CSR.Repositories.Stage
{
    public interface IStageRepository
    {
        public Task<List<Stage>> GetStageListAsync();
        public Task<Stage> GetStageAsync(int stageId);
        public Task<Stage> GetStageAsync(int chapterId, int stageNumber);
        public Task<List<StageEnemy>> GetStageEnemyListAsync(int stageId);
    }

    public class StageRepository : IStageRepository
    {
        private readonly QueryFactory _db;

        public StageRepository(QueryFactory db)
        {
            _db = db;
        }

        public async Task<List<Stage>> GetStageListAsync()
        {
            var stages = await _db.Query("stages")
                                  .GetAsync<Stage>();

            return stages.ToList();
        }

        public async Task<Stage> GetStageAsync(int stageId)
        {
            var stage = await _db.Query("stages")
                                 .Where("id", stageId)
                                 .FirstOrDefaultAsync<Stage>();

            return stage;
        }

        public async Task<Stage> GetStageAsync(int chapterId, int stageNumber)
        {
            var stage = await _db.Query("stages")
                                 .Where("chapter", chapterId)
                                 .Where("stage_number", stageNumber)
                                 .FirstOrDefaultAsync<Stage>();
            return stage;
        }

        public async Task<List<StageEnemy>> GetStageEnemyListAsync(int stageId)
        {
            var stageEnemies = await _db.Query("stage_enemies")
                                        .Where("stage_id", stageId)
                                        .GetAsync<StageEnemy>();
            return stageEnemies.ToList();
        }
    }
}
