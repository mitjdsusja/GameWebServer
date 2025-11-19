using WebServerProject.Models.DTOs.User;

namespace WebServerProject.Models.Response
{
    public class UserInfoResponse
    {
        public bool success { get; set; }
        public string message { get; set; }

        public UserSafeDTO user { get; set; }
    }
}
