using WebServerProject.Models.Entities.Master;

namespace WebServerProject.Models.Entities.User
{
    public class UserCharacter
    {
        public int id { get; set; }
        public int account_id { get; set; }
        public int template_id { get; set; }
        public int level { get; set; } = 1;
        public int experience { get; set; } = 0;
        public int stars { get; set; } = 1;
        public DateTime obtainedAt { get; set; } = DateTime.UtcNow;
    }
}
