using WebServerProject.Models.DTOs.UserEntity;

namespace WebServerProject.Models.DTOs.User
{
    // 클라이언트에 전송할 안전한 사용자 모델 (패스워드 정보 제외)
    public class UserSafeDTO
    {
        public int id { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime? lastLoginAt { get; set; }
        public int status { get; set; }

        public UserStatsDTO stat { get; set; }
        public UserProfilesDTO profile { get; set; }
        public UserResourcesDTO resource { get; set; }

        public static UserSafeDTO FromUser(Entities.UserEntity.User user)
        {
            var model = new UserSafeDTO
            {
                id = user.id,
                username = user.username,
                email = user.email,
                createdAt = user.created_at,
                lastLoginAt = user.last_login_at,
                status = user.status
            };
            return model;
        }
    }
}
