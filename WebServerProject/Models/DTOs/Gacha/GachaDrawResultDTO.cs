using WebServerProject.Models.DTOs.Gacha;
using WebServerProject.Models.DTOs.User;

namespace WebServerProject.Models.Gacha
{
    public class GachaDrawResultDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public GachaPoolDTO? DrawnItem { get; set; }
        public UserResourcesDTO? RemainingResources { get; set; }
    }
}
