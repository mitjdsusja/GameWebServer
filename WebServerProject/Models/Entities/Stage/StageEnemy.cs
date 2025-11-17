namespace WebServerProject.Models.Entities.Stage
{
    public class StageEnemy
    {
        public int id { get; set; }
        public int stage_id { get; set; }
        public int enemy_id { get; set; }
        public int count { get; set; } // 해당 적이 몇 마리 등장?
    }
}
