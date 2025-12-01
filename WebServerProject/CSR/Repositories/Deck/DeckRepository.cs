using SqlKata.Execution;

using DeckEntity =  WebServerProject.Models.Entities.DeckEntity.Deck;
using DeckSlotEntity = WebServerProject.Models.Entities.DeckEntity.DeckSlot;

namespace WebServerProject.CSR.Repositories.Deck
{
    public interface IDeckRepository
    {
        public Task<DeckEntity> GetDeckAsync(int userId, int deckIndex);
        public Task<List<DeckEntity>> GetDeckListAsync(int userId);
        public Task<List<DeckSlotEntity>> GetDeckSlotsAsync(int deckId);

        public Task DeleteDeckSlotsAsync(int deckId);
        public Task InsertDeckSlotAsync(DeckSlotEntity slot);
    }

    public class DeckRepository : IDeckRepository
    {
        private readonly QueryFactory _db;

        public DeckRepository(QueryFactory db)
        {
            _db = db;
        }

        public async Task<DeckEntity> GetDeckAsync(int userId, int deckIndex)
        {
            var deck = await _db.Query("decks")
                                .Where("user_id", userId)
                                .Where("deck_index", deckIndex)
                                .FirstOrDefaultAsync<DeckEntity>();

            return deck;
        }

        public async Task<List<DeckEntity>> GetDeckListAsync(int userId)
        {
            var deck = await _db.Query("decks")
                          .Where("user_id", userId)
                          .GetAsync<DeckEntity>();

            return deck.ToList();
        }

        public async Task<List<DeckSlotEntity>> GetDeckSlotsAsync(int deckId)
        {
            var deckSlots = await _db.Query("deck_slots")
                                     .Where("deck_id", deckId)
                                     .OrderBy("slot_order")
                                     .GetAsync<DeckSlotEntity>();

            return deckSlots.ToList();
        }

        public async Task DeleteDeckSlotsAsync(int deckId)
        {
            await _db.Query("deck_slots")
                     .Where("deck_id", deckId)
                     .DeleteAsync();
        }

        public async Task InsertDeckSlotAsync(DeckSlotEntity slot)
        {
            await _db.Query("deck_slots")
                     .InsertAsync(new
                     {
                         slot.deck_id,
                         slot.user_character_id,
                         slot.slot_order
                     });
        }
    }
}
