namespace WebServerProject.Models.Entities.User
{
    public class User
    {
        public int id { get; set; }
        public string username { get; set; } = null!;
        public string email { get; set; } = null!;
        public string password_hash { get; set; } = null!;
        public string salt { get; set; } = null!;
        public DateTime created_at { get; set; }
        public DateTime? last_login_at { get; set; }
        public string status { get; set; } = "active";
    }
}
