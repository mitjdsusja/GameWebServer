namespace WebServerProject.Models.Entities.Gacha
{
    public class GachaRarityRate
    {
        public int id { get; set; }
        public int gacha_id { get; set; }
        public int rarity { get; set; } // 1-일반, 2-고급, 3-희귀, 4-전설  
        public double rate { get; set; } // 확률 퍼센트 (예: 0.75 = 0.75%)
    }
}
