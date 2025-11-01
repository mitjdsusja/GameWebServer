namespace WebServerProject.Models.Auth
{
    public class RegisterResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int? UserId { get; set; }
    }

    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
        public int? UserId { get; set; }
        public string Username { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }

    public class LogoutResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class ChangePasswordResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
