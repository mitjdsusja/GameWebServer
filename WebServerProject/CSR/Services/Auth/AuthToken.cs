namespace WebServerProject.Models.Utils.Auth
{
    public class AuthToken
    {
        public string Token { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsExpired => DateTime.UtcNow > ExpiresAt;
    }
}
