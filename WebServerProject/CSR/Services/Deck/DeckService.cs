using SqlKata.Execution;
using System.Data;
using WebServerProject.CSR.Repositories.Character;
using WebServerProject.CSR.Repositories.Deck;
using WebServerProject.Models.DTOs.Character;
using WebServerProject.Models.DTOs.Deck;
using WebServerProject.Models.Entities.CharacterEntity;
using WebServerProject.Models.Entities.DeckEntity;

namespace WebServerProject.CSR.Services.Deck
{
    public interface IDeckService
    {
        public Task<DeckDTO> GetDeckAsync(int userId, int deckIndex);
        public Task<List<DeckDTO>> GetDeckListAsync(int userId);
        public Task<DeckDTO> UpdateDeckAsync(int userId, int deckId, List<int> characterId);
        public Task CreateDefaultDecksAsync(int userId, QueryFactory? db = null, IDbTransaction? tx = null);
    }

    public class DeckService : IDeckService
    {
        private readonly QueryFactory _db;
        private readonly IDeckRepository _deckRepository;
        private readonly ICharacterRepository _characterRepository;

        public DeckService(
            QueryFactory db,
            IDeckRepository deckRepository,
            ICharacterRepository characterRepository)
        {
            _db = db;
            _deckRepository = deckRepository;
            _characterRepository = characterRepository;
        }

        public async Task<DeckDTO> GetDeckAsync(int userId, int deckIndex)
        {
            var deck = await _deckRepository.GetDeckAsync(userId, deckIndex);
            if(deck == null)
            {
                throw new InvalidOperationException("덱이 존재하지 않습니다.");
            }

            DeckDTO deckDTO = DeckDTO.FromDeck(deck);

            // 슬롯 정보 가져오기
            var deckSlots = await _deckRepository.GetDeckSlotsAsync(deck.Id);
            if (deckSlots == null)
            {
                throw new InvalidOperationException($"덱({deck.Id}) 슬롯 정보를 불러올 수 없습니다.");
            }

            // 슬롯 DTO 빌드
            List<DeckSlotDTO> deckSlotsDTO = await BuildDeckSlotDTOsAsync(deckSlots);

            deckDTO.deckSlots = deckSlotsDTO;

            return deckDTO;
        }

        public async Task<List<DeckDTO>> GetDeckListAsync(int userId)
        { 
            var decks = await _deckRepository.GetDeckListAsync(userId);
            if(decks == null || decks.Count == 0)
            {
                return new List<DeckDTO>();
            }

            var deckIds = decks.Select(d => d.Id).ToList();
            var allSlots = await _deckRepository.GetDeckSlotsByDeckIdsAsync(deckIds);

            var slotByDeckId = allSlots
                                    .GroupBy(s => s.deck_id)
                                    .ToDictionary(g => g.Key, g => g.OrderBy(s => s.slot_order).ToList());
            var userCharacterIds = allSlots
                                    .Where(s => s.user_character_id.HasValue)
                                    .Select(s => s.user_character_id!.Value)
                                    .Distinct()
                                    .ToList();

            Dictionary<int, UserCharacter> userCharacterMap = new Dictionary<int, UserCharacter>();
            Dictionary<int, CharacterTemplate> characterTemplateMap = new Dictionary<int, CharacterTemplate>();

            if (userCharacterIds.Count > 0)
            {
                // 유저 캐릭터 정보
                var userCharacters = await _characterRepository.GetUserCharacterListAsync(userCharacterIds);
                userCharacterMap = userCharacters.ToDictionary(uc => uc.id, uc => uc);

                // 캐릭터 템플릿 정보
                var templateIds = userCharacters.Select(uc => uc.template_id).Distinct().ToList();
                var characterTemplates = await _characterRepository.GetCharacterTemplateListAsync(templateIds);
                characterTemplateMap = characterTemplates.ToDictionary(ct => ct.id, ct => ct);
            }

            // DTO 조립
            var resultDeckDTOs = new List<DeckDTO>(decks.Count);

            foreach (var deck in decks)
            {
                var deckDTO = DeckDTO.FromDeck(deck);
                if(!slotByDeckId.TryGetValue(deck.Id, out var slots))
                {
                    slots = new List<DeckSlot>();
                }
                var slotDTOs = new List<DeckSlotDTO>(slots.Count);

                // 슬롯 정보
                foreach (var slot in slots)
                {
                    var slotDTO = new DeckSlotDTO
                    {
                        slotIndex = slot.slot_order
                    };

                    // 캐릭터 없는 슬롯
                    if (!slot.user_character_id.HasValue)
                    {
                        slotDTO.characterDetail = null;
                        slotDTOs.Add(slotDTO);
                        continue;
                    }

                    var ucId = slot.user_character_id.Value;

                    // 캐릭터 있는 슬롯: 맵에서 안전하게 조회
                    if (!userCharacterMap.TryGetValue(slot.user_character_id.Value, out var userChar))
                    {
                        Console.WriteLine($"[DEBUG] missing userChar: ucId={ucId}");
                        slotDTO.characterDetail = null;
                        slotDTOs.Add(slotDTO);
                        continue;
                    }

                    if (!characterTemplateMap.TryGetValue(userChar.template_id, out var template))
                    {
                        Console.WriteLine($"[DEBUG] missing template: templateId={userChar.template_id}");
                        slotDTO.characterDetail = null;
                        slotDTOs.Add(slotDTO);
                        continue;
                    }

                    slotDTO.characterDetail = new CharacterDetailDTO
                    {
                        userCharacter = UserCharacterDTO.FromUserCharacter(userChar),
                        characterTemplate = CharacterTemplateDTO.FromCharacterTemplate(template),
                    };

                    slotDTOs.Add(slotDTO);
                }

                deckDTO.deckSlots = slotDTOs;
                resultDeckDTOs.Add(deckDTO);
            }

            return resultDeckDTOs;
        }

        public async Task<DeckDTO> UpdateDeckAsync(int userId, int deckIndex, List<int> userCharacterIdList)
        {
            // 덱 검증
            var deck = await _deckRepository.GetDeckAsync(userId, deckIndex);
            if (deck == null)
            {
                throw new InvalidOperationException("덱이 존재하지 않습니다.");
            }
            else if(deck.user_id != userId)
            {
                throw new InvalidOperationException("해당 덱은 유저의 소유가 아닙니다.");
            }

            // 슬롯 개수 검증
            if (userCharacterIdList == null || userCharacterIdList.Count == 0)
            {
                throw new InvalidOperationException("업데이트할 덱 슬롯 정보가 없습니다.");
            }
            if (userCharacterIdList.Count > 5)
            {
                throw new InvalidOperationException("덱 슬롯 개수는 최대 5개입니다.");
            }

            // 캐릭터 소유권 검증
            foreach (var userCharacterId in userCharacterIdList)
            {
                if (userCharacterId <= 0)
                    continue;

                var userCharacter = await _characterRepository.GetUserCharacterAsync(userId, userCharacterId);
                if (userCharacter == null)
                {
                    throw new InvalidOperationException($"해당 유저는 캐릭터 {userCharacterId}를 소유하고 있지 않습니다.");
                }
                else if (userCharacter.user_id != userId)
                {
                    throw new InvalidOperationException($"캐릭터 {userCharacterId}는 해당 유저의 것이 아닙니다.");
                }
            }

            var conn = _db.Connection;
            conn.Open();
            using var tx = conn.BeginTransaction();

            try
            {
                // 기존 슬롯 삭제
                await _deckRepository.DeleteDeckSlotsAsync(deck.Id, _db, tx);

                // 새로운 슬롯 삽입
                for (int i = 0; i < userCharacterIdList.Count; i++)
                {
                    int slotOrder = i + 1;
                    int? userCharacterId = userCharacterIdList[i] > 0 ? userCharacterIdList[i] : null;

                    var newSlot = new DeckSlot
                    {
                        deck_id = deck.Id,
                        slot_order = (byte)slotOrder,
                        user_character_id = userCharacterId
                    };

                    await _deckRepository.InsertDeckSlotAsync(newSlot, _db, tx);
                }

                tx.Commit();
            }
            catch (Exception)
            {
                tx.Rollback();
                throw;
            }

            // 갱신된 덱 정보를 읽어와 DTO로 반환
            var updatedDeck = await GetDeckAsync(userId, deckIndex); // 너의 기존 GetDeckList 로직과 동일

            return updatedDeck;
        }

        // TODO : 최적화 필요 
        // 현재 슬롯 마다 캐릭터 정보와 템플릿 정보를 별도로 조회하고 있음.
        // 추후 한번에 묶어서 조회하는 방식으로 변경 필요.
        private async Task<List<DeckSlotDTO>> BuildDeckSlotDTOsAsync(List<DeckSlot> deckSlots)
        {
            List<DeckSlotDTO> result = new List<DeckSlotDTO>();

            foreach (var slot in deckSlots)
            {
                var slotDTO = new DeckSlotDTO
                {
                    slotIndex = slot.slot_order
                };

                // 빈 슬롯
                if (slot.user_character_id == null)
                {
                    slotDTO.characterDetail = null;
                    result.Add(slotDTO);
                    continue;
                }

                var userCharacter = await _characterRepository.GetUserCharacterAsync((int)slot.user_character_id);
                if (userCharacter == null)
                {
                    throw new InvalidOperationException($"유효하지 않은 캐릭터입니다. (UserCharacterId: {slot.user_character_id})");
                }

                var characterTemplate = await _characterRepository.GetCharacterTemplateAsync(userCharacter.template_id);
                if (characterTemplate == null)
                {
                    throw new InvalidOperationException($"유효하지 않은 캐릭터 템플릿입니다. (TemplateId: {userCharacter.template_id})");
                }

                slotDTO.characterDetail = new CharacterDetailDTO
                {
                    userCharacter = UserCharacterDTO.FromUserCharacter(userCharacter),
                    characterTemplate = CharacterTemplateDTO.FromCharacterTemplate(characterTemplate)
                };

                result.Add(slotDTO);
            }

            return result;
        }

        public async Task CreateDefaultDecksAsync(int userId, QueryFactory? db = null, IDbTransaction? tx = null)
        {
            const int DEFAULT_DECK_COUNT = 3;
            const int DEFAULT_SLOT_COUNT = 3;

            for (int i=0;i< DEFAULT_DECK_COUNT; i++)
            {
                int deckId = await _deckRepository.CreateDeckAsync(new Models.Entities.DeckEntity.Deck
                {
                    user_id = userId,
                    deck_index = i+1,
                }, db, tx);

                for (byte slot = 0; slot < DEFAULT_SLOT_COUNT; slot++)
                {
                    await _deckRepository.CreateDeckSlotAsync(new DeckSlot
                    {
                        deck_id = deckId,
                        user_character_id = null,
                        slot_order = (byte)(slot + 1),
                    }, db, tx);
                }
            }
        }
    }
}
