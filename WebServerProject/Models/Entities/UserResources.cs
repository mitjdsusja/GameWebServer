namespace WebServerProject.Models.Entities
{
    public class UserResources
    {
        public int UserId { get; set; }
        public int Golds { get; set; }
        public int Diamonds { get; set; } = 1;

        // 관계
        public User User { get; set; } = null!;
    }
}
