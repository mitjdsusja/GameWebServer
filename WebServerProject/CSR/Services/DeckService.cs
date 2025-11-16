using WebServerProject.CSR.Repositories;
using WebServerProject.Models.DTOs.Character;
using WebServerProject.Models.DTOs.Deck;

namespace WebServerProject.CSR.Services
{
    public interface IDeckService
    {
        public Task<List<DeckDTO>> GetDeckListAsync(int userId);
    }
    public class DeckService : IDeckService
    {
        private readonly IDeckRepository _deckRepository;
        private readonly ICharacterRepository _characterRepository;

        public DeckService(
            IDeckRepository deckRepository,
            ICharacterRepository characterRepository)
        {
            _deckRepository = deckRepository;
            _characterRepository = characterRepository;
        }

        // TODO : 덱 목록 조회 최적화 필요 (조인 쿼리 등)
        public async Task<List<DeckDTO>> GetDeckListAsync(int userId)
        {
            var decks = await _deckRepository.GetDeckListAsync(userId);
            if(decks == null)
            {
                throw new InvalidOperationException("덱이 존재하지 않습니다.");
            }

            List<DeckDTO> deckDTOs = new List<DeckDTO>();

            foreach (var deck in decks)
            {
                // DTO 생성
                DeckDTO deckDTO = DeckDTO.FromDeck(deck);
                List<DeckSlotDTO> deckSlotsDTO = new List<DeckSlotDTO>();

                // 슬롯 정보 가져오기
                var deckSlots = await _deckRepository.GetDeckSlotsAsync(deck.Id);
                if (deckSlots == null)
                {
                    throw new InvalidOperationException($"덱({deck.Id}) 슬롯 정보를 불러올 수 없습니다.");
                }

                foreach (var slot in deckSlots)
                {
                    DeckSlotDTO dectSlotDOT = new DeckSlotDTO
                    {
                        slotIndex = slot.slot_order
                    };

                    var userCharacter = await _characterRepository.GetUserCharacterAsync(slot.user_character_id);
                    if (userCharacter == null)
                    {
                        throw new InvalidOperationException($"유효하지 않은 캐릭터입니다. (UserCharacterId: {slot.user_character_id})");
                    }
                    var characterTemplate = await _characterRepository.GetCharacterTemplateAsync(userCharacter.template_id);
                    if (characterTemplate == null)
                    {
                        throw new InvalidOperationException($"유효하지 않은 캐릭터 템플릿입니다. (TemplateId: {userCharacter.template_id})");
                    }

                    dectSlotDOT.CharacterDetailDTO = new CharacterDetailDTO
                    {
                        userCharacter = UserCharacterDTO.FromUserCharacter(userCharacter),
                        characterTemplate = CharacterTemplateDTO.FromCharacterTemplate(characterTemplate)
                    };

                    deckSlotsDTO.Add(dectSlotDOT);
                }
                deckDTO.deckSlots = deckSlotsDTO;

                deckDTOs.Add(deckDTO);
            }

            return deckDTOs;
        }
    }
}
