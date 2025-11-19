namespace WebServerProject.Models.Request.Deck
{
    public class DeckListRequest
    {
        public int userId { get; set; }
    }

    public class DeckUpdateRequest
    {
        public int userId { get; set; }
        public int deckIndex { get; set; }
        public List<int> characterIds { get; set; }
    }
}
