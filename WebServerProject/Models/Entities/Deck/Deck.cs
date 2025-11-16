namespace WebServerProject.Models.Entities.Deck
{
    public class Deck
    {
        public int Id { get; set; }
        public int user_id { get; set; }
        public int deck_index { get; set; }
        public string name { get; set; } = "Default Deck";
        public bool is_active { get; set; } = false;
    }
}
