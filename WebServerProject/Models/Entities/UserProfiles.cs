namespace WebServerProject.Models.Entities
{
    public class UserProfiles
    {
        public int UserId { get; set; }
        public string Nickname { get; set; }
        public string Introduction { get; set; }

        // 관계
        public User User { get; set; } = null!;
    }
}
