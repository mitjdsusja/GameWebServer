using WebServerProject.Models.DTOs.Character;
using WebServerProject.Models.DTOs.User;

namespace WebServerProject.Models.Response.Character
{
    public class CharacterListResponse
    {
        public bool success { get; set; }
        public string message { get; set; }
        public List<CharacterDetailDTO>? characters { get; set; }
    }
}
