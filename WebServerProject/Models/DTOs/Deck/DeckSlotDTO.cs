using WebServerProject.Models.DTOs.Character;

namespace WebServerProject.Models.DTOs.Deck
{
    public class DeckSlotDTO
    {
        public int slotIndex { get; set; }
        public UserCharacterDTO? userCharacter { get; set; } = null;
        public CharacterTemplateDTO? characterTemplate { get; set; } = null;
    }
}
