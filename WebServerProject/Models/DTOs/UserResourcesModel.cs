using WebServerProject.Models.Entities.User;

namespace WebServerProject.Models.DTOs
{
    public class UserResourcesModel
    {
        public int Golds { get; set; }
        public int Diamonds { get; set; }

        public static UserResourcesModel FromUserResources(UserResources resources)
        {
            return new UserResourcesModel
            {
                Golds = resources.Golds,
                Diamonds = resources.Diamonds
            };
        }
    }
}
