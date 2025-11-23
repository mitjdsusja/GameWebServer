using WebServerProject.Models.DTOs.Gacha;

namespace WebServerProject.CSR.Services.Gacha
{
    public class GachaRewardResultDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;

        // 실제로 지급된 아이템/캐릭터 정보
        public GachaPoolDTO? gachaPool { get; set; }

        // 신규인지 여부
        public bool IsNew { get; set; } = false;
    }
}
