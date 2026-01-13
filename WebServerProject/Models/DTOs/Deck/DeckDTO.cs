using DeckEntity = WebServerProject.Models.Entities.DeckEntity.Deck;

namespace WebServerProject.Models.DTOs.Deck
{
    public class DeckDTO
    {
        public int deckIndex { get; set; }
        public string name { get; set; }
        public bool isActive { get; set; }
        public List<DeckSlotDTO>? deckSlots { get; set; } = null;
        
        public static DeckDTO FromDeck(DeckEntity deck)
        {
            var dto = new DeckDTO
            {
                deckIndex = deck.deck_index,
                name = deck.name,
                isActive = deck.is_active
            };

            return dto;
        }
    }
}
