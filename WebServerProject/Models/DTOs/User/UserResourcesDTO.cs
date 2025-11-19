using WebServerProject.Models.Entities.UserEntity;

namespace WebServerProject.Models.DTOs.UserEntity
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
