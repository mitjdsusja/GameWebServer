using WebServerProject.Models.DTOs.Enemy;
using WebServerProject.Models.Entities.StageEntity;

namespace WebServerProject.Models.DTOs.Stage
{
    public class StageEnemyDTO
    {
        public int id { get; set; }
        public int stageId { get; set; }
        public int enemyId { get; set; }
        public int count { get; set; } 

        public EnemyTemplateDTO enemyTemplate { get; set; }

        public static StageEnemyDTO FromStageEnemy(StageEnemy stageEnemy)
        {
            return new StageEnemyDTO
            {
                id = stageEnemy.id,
                stageId = stageEnemy.stage_id,
                enemyId = stageEnemy.enemy_id,
                count = stageEnemy.count
            };
        }
    }
}
