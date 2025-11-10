using WebServerProject.Models.Entities.User;

namespace WebServerProject.Models.DTOs.User
{
    public class UserResourcesDTO
    {
        public int Gold { get; set; }
        public int Diamond { get; set; }

        public static UserResourcesDTO FromUserResources(UserResources resources)
        {
            return new UserResourcesDTO
            {
                Gold = resources.gold,
                Diamond = resources.diamond
            };
        }
    }
}
