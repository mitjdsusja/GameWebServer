
namespace WebServerProject.Models.Entities.Gacha
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
    }
}
