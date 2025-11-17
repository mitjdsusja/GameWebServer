using WebServerProject.Models.DTOs.Deck;

namespace WebServerProject.Models.Response
{
    public class DeckListResponse
    {
        public bool success { get; set; }
        public string message { get; set; }
        public List<DeckDTO> decks { get; set; }
    }

    public class DeckUpdateResponse
    {
        public bool success { get; set; }
        public string message { get; set; }
        public DeckDTO? updatedDeck { get; set; }
    }
}
