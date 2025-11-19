using WebServerProject.Models.Entities.GachaEntity;

namespace WebServerProject.Models.DTOs.Gacha
{
    public class GachaMasterDTO
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

        public static GachaMasterDTO FromGachaMaster(GachaMaster gachaMaster)
        {
            return new GachaMasterDTO
            {
                id = gachaMaster.id,
                code = gachaMaster.code,
                name = gachaMaster.name,
                description = gachaMaster.description,
                cost_type = gachaMaster.cost_type,
                cost_amount = gachaMaster.cost_amount,
                is_limited = gachaMaster.is_limited,
                start_time = gachaMaster.start_time,
                end_time = gachaMaster.end_time
            };
        }
    }
}
