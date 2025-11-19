using SqlKata.Execution;
using WebServerProject.Models.Entities.EnemyEntity;

namespace WebServerProject.CSR.Repositories.Enemy
{
    public interface IEnemyRepository
    {
        public Task<EnemyTemplate> GetEnemyTemplateAsync(int templateId);
    }

    public class EnemyRepository : IEnemyRepository
    {
        private readonly QueryFactory _db;

        public EnemyRepository(QueryFactory db)
        {
            _db = db;
        }

        public async Task<EnemyTemplate> GetEnemyTemplateAsync(int templateId)
        {
            var enemyTemplate = await _db.Query("enemy_templates")
                                         .Where("id", templateId)
                                         .FirstOrDefaultAsync<EnemyTemplate>();

            return enemyTemplate;
        }
    }
}
