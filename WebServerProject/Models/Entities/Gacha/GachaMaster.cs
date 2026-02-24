
namespace WebServerProject.Models.Entities.GachaEntity
{
    public class GachaMaster
    {
        public int id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string? description { get; set; }
        public int cost_type { get; set; }
        public int cost_amount { get; set; }
        public bool is_limited { get; set; }
        public DateTime? start_time { get; set; }
        public DateTime? end_time { get; set; }

        public int soft_pity_threshold { get; set; } = 0;
        public int hard_pity_threshold { get; set; } = 0;
        public double pity_bonus_rate { get; set; } = 0.0;
        public int pity_target_rarity { get; set; } = 0;
    }
}
