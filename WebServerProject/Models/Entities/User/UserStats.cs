namespace WebServerProject.Models.Entities.User
{
    public class UserStats
    {
        public int user_id { get; set; }
        public int level { get; set; } = 1;
        public int exp { get; set; } = 0;
    }
}
