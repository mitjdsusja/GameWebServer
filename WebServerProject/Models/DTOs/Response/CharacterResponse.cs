using WebServerProject.Models.Entities.User;

namespace WebServerProject.Models.DTOs.Response
{
    public class CharacterListResponse
    {
        public bool success { get; set; }
        public string message { get; set; }
        public List<UserCharacter> characters { get; set; }
    }
}
