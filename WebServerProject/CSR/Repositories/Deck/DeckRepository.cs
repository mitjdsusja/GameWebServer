using SqlKata.Execution;
using WebServerProject.Models.DTOs.Deck;
using WebServerProject.Models.Entities.Deck;

namespace WebServerProject.CSR.Repositories.Deck
{
    public interface IDeckRepository
    {
        public Task<Deck> GetDeckAsync(int deckId);
        public Task<List<Deck>> GetDeckListAsync(int userId);
        public Task<List<DeckSlot>> GetDeckSlotsAsync(int deckId);

        public Task DeleteDeckSlotsAsync(int deckId);
        public Task InsertDeckSlotAsync(DeckSlot slot);
    }

    public class DeckRepository : IDeckRepository
    {
        private readonly QueryFactory _db;

        public DeckRepository(QueryFactory db)
        {
            _db = db;
        }

        public async Task<Deck> GetDeckAsync(int deckId)
        {
            var deck = await _db.Query("decks")
                                .Where("id", deckId)
                                .FirstOrDefaultAsync<Deck>();

            return deck;
        }

        public async Task<List<Deck>> GetDeckListAsync(int userId)
        {
            var deck = await _db.Query("decks")
                          .Where("user_id", userId)
                          .GetAsync<Deck>();

            return deck.ToList();
        }

        public async Task<List<DeckSlot>> GetDeckSlotsAsync(int deckId)
        {
            var deckSlots = await _db.Query("deck_slots")
                                     .Where("deck_id", deckId)
                                     .OrderBy("slot_order")
                                     .GetAsync<DeckSlot>();

            return deckSlots.ToList();
        }

        public async Task DeleteDeckSlotsAsync(int deckId)
        {
            await _db.Query("deck_slots")
                     .Where("deck_id", deckId)
                     .DeleteAsync();
        }

        public async Task InsertDeckSlotAsync(DeckSlot slot)
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
