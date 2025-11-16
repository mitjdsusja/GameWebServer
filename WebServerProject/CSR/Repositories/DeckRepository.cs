using SqlKata.Execution;
using WebServerProject.Models.DTOs.Deck;
using WebServerProject.Models.Entities.Deck;

namespace WebServerProject.CSR.Repositories
{
    public interface IDeckRepository
    {
        public Task<List<Deck>> GetDeckListAsync(int userId);
        public Task<List<DeckSlot>> GetDeckSlotsAsync(int deckId);
    }

    public class DeckRepository : IDeckRepository
    {
        private readonly QueryFactory _db;

        public DeckRepository(QueryFactory db)
        {
            _db = db;
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
    }
}
