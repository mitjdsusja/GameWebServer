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

            // 슬롯 정보 가져오기
            var deckSlots = await _deckRepository.GetDeckSlotsAsync(deck.Id);
            if (deckSlots == null)
            {
                throw new InvalidOperationException($"덱({deck.Id}) 슬롯 정보를 불러올 수 없습니다.");
            }

            // 캐릭터 정보
            List<int> userCharacterIds = deckSlots
                                            .Where(s => s.user_character_id.HasValue)
                                            .Select(s => s.user_character_id!.Value)
                                            .Distinct()
                                            .ToList();
            var userChars = await _characterRepository.GetUserCharacterListAsync(userCharacterIds);
            Dictionary<int, UserCharacter> userCharsByUserCharId = userChars.ToDictionary(uc => uc.id, uc => uc);

            // 캐릭터 템플릿 정보
            List<int> templateIds = userChars
                                    .Select(uc => uc.template_id)
                                    .Distinct()
                                    .ToList();
            var templates = await _characterRepository.GetCharacterTemplateListAsync(templateIds);
            Dictionary<int, CharacterTemplate> templatesByTemplateId = templates.ToDictionary(t => t.id, t => t);

            // 슬롯 DTO 빌드
            DeckDTO deckDTO = DeckDTOFactory.Create(deck, deckSlots, userCharsByUserCharId, templatesByTemplateId);

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

            // DeckDTO 생성
            var result = new List<DeckDTO>();
            foreach (var deck in decks)
            {
                slotByDeckId.TryGetValue(deck.Id, out var slots);
                result.Add(DeckDTOFactory.Create(deck, slots, userCharacterMap, characterTemplateMap));
            }

            return result;
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

            var updatedDeck = await GetDeckAsync(userId, deckIndex);

            return updatedDeck;
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
