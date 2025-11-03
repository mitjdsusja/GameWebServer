namespace WebServerProject.Models.DTOs
{
    public class UserStatsModel
    {
        public int UserId { get; set; }
        public int Level { get; set; }
        public int Exp { get; set; }

        public static UserStatsModel FromUserStats(Entities.UserStats stats)
        {
            return new UserStatsModel
            {
                UserId = stats.UserId,
                Level = stats.Level,
                Exp = stats.Exp
            };
        }
    }
}
