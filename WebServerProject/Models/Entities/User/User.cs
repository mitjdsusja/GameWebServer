namespace WebServerProject.Models.Entities.UserEntity
{
    public class User
    {
        public enum UserStatus
        {
            Inactive = 0,
            Active = 1,
            Banned = 2
        }
        public int id { get; set; }
        public string username { get; set; } = null!;
        public string email { get; set; } = null!;
        public string password_hash { get; set; } = null!;
        public string salt { get; set; } = null!;
        public DateTime created_at { get; set; }
        public DateTime? last_login_at { get; set; }
        public int status { get; set; }
    }
}
