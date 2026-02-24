namespace WebServerProject.Models.Entities.Gacha
{
    public class UserGachaPity
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public int gacha_id { get; set; }
        public int pity_stack { get; set; } = 0;
        public DateTime updated_at { get; set; }
    }
}
