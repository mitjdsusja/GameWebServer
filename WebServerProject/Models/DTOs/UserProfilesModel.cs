using WebServerProject.Models.Entities.User;

namespace WebServerProject.Models.DTOs
{
    public class UserProfilesModel
    {
        public string Nickname { get; set; }
        public string Introduction { get; set; }

        public static UserProfilesModel FromUserProfiles(UserProfiles profile)
        {
            return new UserProfilesModel
            {
                Nickname = profile.Nickname,
                Introduction = profile.Introduction,
            };
        }
    }
}
