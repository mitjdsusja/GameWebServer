namespace WebServerProject.Models.Entities
{
    public class UserStats
    {
        public int UserId { get; set; }
        public int Level { get; set; } = 1;
        public int Exp { get; set; } = 0;

        // 관계
        public User User { get; set; } = null!;
    }
}
