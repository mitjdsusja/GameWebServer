using WebServerProject.Models.Entities.Enemy;

namespace WebServerProject.Models.DTOs.Enemy
{
    public class EnemyTemplateDTO
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int level { get; set; }
        public int hp { get; set; }
        public int attack { get; set; }
        public int defense { get; set; }

        public static EnemyTemplateDTO FromEnemyTemplate(EnemyTemplate enemyTemplate)
        {
            return new EnemyTemplateDTO
            {
                id = enemyTemplate.id,
                name = enemyTemplate.name,
                description = enemyTemplate.description,
                level = enemyTemplate.level,
                hp = enemyTemplate.hp,
                attack = enemyTemplate.attack,
                defense = enemyTemplate.defense
            };
        }
    }
}
