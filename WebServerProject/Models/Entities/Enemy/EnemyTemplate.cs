namespace WebServerProject.Models.Entities.EnemyEntity
{
    public class EnemyTemplate
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int level { get; set; }
        public int hp { get; set; }
        public int attack { get; set; }
        public int defense { get; set; }
    }
}
