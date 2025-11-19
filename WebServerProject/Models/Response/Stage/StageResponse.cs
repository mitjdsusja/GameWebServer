using WebServerProject.Models.DTOs.Stage;

namespace WebServerProject.Models.Response.Stage
{
    public class StageListResponse
    {
        public bool success { get; set; }
        public string message { get; set; }
        public List<StageDTO>? stages { get; set; }
    }
}
