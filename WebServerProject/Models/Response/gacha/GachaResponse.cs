using WebServerProject.Models.DTOs.Gacha;
using WebServerProject.Models.DTOs.User;

namespace WebServerProject.Models.Response.gacha
{
    public class  GachaListResponse
    {
        public bool success { get; set; }  
        public string message { get; set; }
        public List<GachaMasterDTO>? gachaList { get; set; }
    }

    public class GachaDrawResponse
    {
        public bool success { get; set; }
        public string message { get; set; }

        public GachaPoolDTO? drawnItem { get; set; }
        public UserResourcesDTO? remainingResources { get; set; }
    }
}
