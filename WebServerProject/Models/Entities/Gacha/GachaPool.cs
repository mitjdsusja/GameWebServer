using Humanizer;
using static Humanizer.In;

namespace WebServerProject.Models.Entities.Gacha
{
    public class GachaPool
    {
        public enum ItemType
        {
            ITEM_CHARACTER = 1,
            ITEM_EQUIPMENT = 2,
            ITEM_CONSUMABLE = 3
        }
        public int id { get; set; }
        public int gacha_id { get; set; }
        public int item_type { get; set; } // 1-캐릭터, 2-장비, 3-아이템
        public int item_id { get; set; }
        public int rarity { get; set; } // 1-일반, 2-고급, 3-희귀, 4-전설
    }
}
