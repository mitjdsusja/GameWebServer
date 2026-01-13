using WebServerProject.Models.DTOs.Character;
using WebServerProject.Models.Entities.CharacterEntity;
using WebServerProject.Models.Entities.DeckEntity;
using DeckEntity = WebServerProject.Models.Entities.DeckEntity.Deck;

namespace WebServerProject.Models.DTOs.Deck
{
    public static class DeckDTOFactory
    {
        public static DeckDTO Create(
            DeckEntity deck,
            List<DeckSlot>? deckSlots = null,
            Dictionary<int, UserCharacter>? userCharByUserCharacterId = null,
            Dictionary<int, CharacterTemplate>? templateByTemplateId = null
            )
        {
            var result = DeckDTO.FromDeck(deck);

            // Exception Check
            // No Slots
            if(deckSlots == null || deckSlots.Count == 0)
            {
                result.deckSlots = new List<DeckSlotDTO>();
                return result;
            }
            // No UserCharacters or Templates
            bool hasCharacterSlot = deckSlots.Any(s => s.user_character_id.HasValue);
            if(hasCharacterSlot && (userCharByUserCharacterId == null || templateByTemplateId == null))
            {
                throw new Exception($"DeckDTOFactory.Create: character slots exist but maps are null. deckId={deck.Id}");
            }

            // Fill DeckSlots
            result.deckSlots = new List<DeckSlotDTO>(deckSlots.Count);
            foreach(var slot in deckSlots.OrderBy(s => s.slot_order))
            {
                var slotDTO = new DeckSlotDTO
                {
                    slotIndex = slot.slot_order,
                    userCharacter = null,
                    characterTemplate = null,
                };

                // Empty Slot
                if (slot.user_character_id.HasValue == false)
                {
                    result.deckSlots.Add(slotDTO);
                    continue;
                }

                int curUserCharId = slot.user_character_id.Value;
                // User Character
                if (userCharByUserCharacterId.TryGetValue(curUserCharId, out var userChar) == false)
                {
                    throw new InvalidOperationException($"UserCharacter not found. userCharacterId={curUserCharId}, deckId={deck.Id}, slotOrder={slot.slot_order}");
                }

                // Character Template
                if(templateByTemplateId.TryGetValue(userChar.template_id, out var template) == false)
                {
                    throw new InvalidOperationException($"Template not found. templateId={userChar.template_id}, userCharacterId={curUserCharId}, deckId={deck.Id}");
                }

                slotDTO.userCharacter = UserCharacterDTO.FromUserCharacter(userChar);
                slotDTO.characterTemplate = CharacterTemplateDTO.FromCharacterTemplate(template); 

                result.deckSlots.Add(slotDTO);      
            }

            return result;
        }
    }
}
