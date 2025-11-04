using WebServerProject.Models.Entities.User;

namespace WebServerProject.Models.DTOs
{
    public class UserStatsModel
    {
        public int Level { get; set; }
        public int Exp { get; set; }

        public static UserStatsModel FromUserStats(UserStats stats)
        {
            return new UserStatsModel
            {
                Level = stats.Level,
                Exp = stats.Exp
            };
        }
    }
}
