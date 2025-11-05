namespace WebServerProject.Models.Entities.User
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string Salt { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public string Status { get; set; } = "active";

        public UserStats Stats { get; set; } = null!;
        public UserResources Resources { get; set; } = null!;
        public UserProfiles Profile { get; set; } = null!;
    }
}
