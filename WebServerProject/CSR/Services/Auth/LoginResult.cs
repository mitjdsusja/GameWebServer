using WebServerProject.Models.Utils.Auth;
using static Humanizer.In;

namespace WebServerProject.CSR.Services.Auth
{
    public class LoginResult
    {
        public bool success { get; set; }
        public string message { get; set; }
        public AuthToken? token { get; set; }
    }
}
