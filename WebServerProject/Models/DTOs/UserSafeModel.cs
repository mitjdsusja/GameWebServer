using WebServerProject.Models.Entities;

namespace WebServerProject.Models.DTOs
{
    // 클라이언트에 전송할 안전한 사용자 모델 (패스워드 정보 제외)
    public class UserSafeModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public string Status { get; set; }

        public static UserSafeModel FromUser(User user)
        {
            return new UserSafeModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                Status = user.Status
            };
        }
    }
}
