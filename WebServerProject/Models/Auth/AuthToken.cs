namespace WebServerProject.Models.Auth
{
    public class AuthToken
    {
        public string Token { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public string DeviceId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsExpired => DateTime.UtcNow > ExpiresAt;
    }
}
