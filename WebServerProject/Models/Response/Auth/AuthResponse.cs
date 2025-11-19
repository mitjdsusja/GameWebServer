namespace WebServerProject.Models.Response
{
    public class RegisterResponse
    {
        public bool success { get; set; }
        public string message { get; set; }
        public int? userId { get; set; }
    }

    public class LoginResponse
    {
        public bool success { get; set; }
        public string message { get; set; }
        public string token { get; set; }
        public int? userId { get; set; }
        public string sername { get; set; }
        public DateTime? expiresAt { get; set; }
    }

    public class LogoutResponse
    {
        public bool success { get; set; }
        public string message { get; set; }
    }

    public class ChangePasswordResponse
    {
        public bool success { get; set; }
        public string message { get; set; }
    }
}
