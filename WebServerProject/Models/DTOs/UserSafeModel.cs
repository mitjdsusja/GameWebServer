using WebServerProject.Models.Entities.User;

namespace WebServerProject.Models.DTOs
{
    // 클라이언트에 전송할 안전한 사용자 모델 (패스워드 정보 제외)
    public class UserSafeModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public string Status { get; set; }

        public static UserSafeModel FromUser(User user)
        {
            var model = new UserSafeModel
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
