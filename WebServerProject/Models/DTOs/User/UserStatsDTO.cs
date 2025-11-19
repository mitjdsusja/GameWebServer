using WebServerProject.Models.Entities.UserEntity;

namespace WebServerProject.Models.DTOs.User
{
    public class UserStatsDTO
    {
        public int Level { get; set; }
        public int Exp { get; set; }

        public static UserStatsDTO FromUserStats(UserStats stats)
        {
            return new UserStatsDTO
            {
                Level = stats.level,
                Exp = stats.exp
            };
        }
    }
}
