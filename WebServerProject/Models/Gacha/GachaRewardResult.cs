namespace WebServerProject.Models.Gacha
{
    public class GachaRewardResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;

        // 실제로 지급된 아이템/캐릭터 정보
        public int ItemType { get; set; }
        public int ItemId { get; set; }
        public int Rarity { get; set; }

        // 신규인지 여부
        public bool IsNew { get; set; }
    }
}
