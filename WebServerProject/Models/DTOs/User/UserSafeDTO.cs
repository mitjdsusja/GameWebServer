namespace WebServerProject.Models.DTOs.User
{
    // 클라이언트에 전송할 안전한 사용자 모델 (패스워드 정보 제외)
    public class UserSafeDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public string Status { get; set; }

        public static UserSafeDTO FromUser(Entities.User.User user)
        {
            var model = new UserSafeDTO
            {
                Id = user.id,
                Username = user.username,
                Email = user.email,
                CreatedAt = user.created_at,
                LastLoginAt = user.last_login_at,
                Status = user.status
            };
            return model;
        }
    }
}
