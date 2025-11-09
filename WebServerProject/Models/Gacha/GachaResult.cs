using WebServerProject.Models.Entities.Gacha;
using WebServerProject.Models.Entities.Master;

namespace WebServerProject.Models.Gacha
{
    public class GachaDrawResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public GachaPool? DrawnItem { get; set; }
        public int RemainingDiamonds { get; set; }
    }
}
