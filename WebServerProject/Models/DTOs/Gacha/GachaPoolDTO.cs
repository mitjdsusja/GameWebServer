using WebServerProject.Models.Entities.GachaEntity;

namespace WebServerProject.Models.DTOs.Gacha
{
    public class GachaPoolDTO
    {
        public int Id { get; set; }
        public int ItemType { get; set; }
        public int ItemId { get; set; }
        public int Rarity { get; set; }

        public static GachaPoolDTO FromGachaPool(GachaPool pool)
        {
            return new GachaPoolDTO
            {
                Id = pool.id,
                ItemType = pool.item_type,
                ItemId = pool.item_id,
                Rarity = pool.rarity
            };
        }
    }
}
