using WebServerProject.Models.DTOs.Character;

namespace WebServerProject.Models.DTOs.Deck
{
    public class DeckSlotDTO
    {
        public int slotIndex { get; set; }
        public CharacterDetailDTO? characterDetail { get; set; }
    }
}
