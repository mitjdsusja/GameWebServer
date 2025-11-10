using WebServerProject.Models.Entities.User;

namespace WebServerProject.Models.DTOs.User
{
    public class UserResourcesDTO
    {
        public int Golds { get; set; }
        public int Diamonds { get; set; }

        public static UserResourcesDTO FromUserResources(UserResources resources)
        {
            return new UserResourcesDTO
            {
                Golds = resources.golds,
                Diamonds = resources.diamonds
            };
        }
    }
}
