namespace WebServerProject.Models.Entities.DeckEntity
{
    public class DeckSlot
    {
        public int id { get; set; }
        public int deck_id { get; set; }
        public int? user_character_id { get; set; }
        public byte slot_order { get; set; }    
    }
}
