namespace WebServerProject.Models.DTOs.Response
{
    public class UserInfoResponse
    {
        public bool success { get; set; }
        public string message { get; set; }

        public UserSafeModel user { get; set; }
        public UserStatsModel stats { get; set; }
        public UserProfilesModel profiles { get; set; }
        public UserResourcesModel resources { get; set; }
    }
}
