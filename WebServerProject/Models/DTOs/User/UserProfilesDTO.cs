using WebServerProject.Models.Entities.User;

namespace WebServerProject.Models.DTOs.User
{
    public class UserProfilesDTO
    {
        public string Nickname { get; set; }
        public string Introduction { get; set; }

        public static UserProfilesDTO FromUserProfiles(UserProfiles profile)
        {
            return new UserProfilesDTO
            {
                Nickname = profile.nickname,
                Introduction = profile.introduction,
            };
        }
    }
}
