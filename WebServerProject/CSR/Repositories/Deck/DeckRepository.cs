using Dapper;
using SqlKata;
using SqlKata.Execution;
using System.Data;
using DeckEntity =  WebServerProject.Models.Entities.DeckEntity.Deck;
using DeckSlotEntity = WebServerProject.Models.Entities.DeckEntity.DeckSlot;

namespace WebServerProject.CSR.Repositories.Deck
{
    public interface IDeckRepository
    {
        public Task<DeckEntity> GetDeckAsync(int userId, int deckIndex, IDbTransaction? tx = null);
        public Task<DeckEntity> GetDeckForUpdateAsync(int userId, int deckIndex, IDbTransaction? tx = null);
        public Task<List<DeckEntity>> GetDeckListAsync(int userId, IDbTransaction? tx = null);
        public Task<List<DeckSlotEntity>> GetDeckSlotsAsync(int deckId, IDbTransaction? tx = null);
        public Task<List<DeckSlotEntity>> GetDeckSlotsByDeckIdsAsync(List<int> deckIds, IDbTransaction? tx = null);

        public Task DeleteDeckSlotsAsync(int deckId, IDbTransaction? tx = null);
        public Task InsertDeckSlotAsync(DeckSlotEntity slot, IDbTransaction? tx = null);

        public Task<int> CreateDeckAsync(DeckEntity deck, IDbTransaction? tx = null);
        public Task CreateDeckSlotAsync(DeckSlotEntity deckSlot, IDbTransaction? tx = null);
    }

    public class DeckRepository : IDeckRepository
    {
        private readonly QueryFactory _db;

        public DeckRepository(QueryFactory db)
        {
            _db = db;
        }

        public async Task<DeckEntity> GetDeckAsync(int userId, int deckIndex, IDbTransaction? tx = null)
        {
            var deck = await _db.Query("decks")
                                .Where("user_id", userId)
                                .Where("deck_index", deckIndex)
                                .FirstOrDefaultAsync<DeckEntity>();

            return deck;
        }

        public async Task<List<DeckEntity>> GetDeckListAsync(int userId, IDbTransaction? tx = null)
        {
            var deck = await _db.Query("decks")
                          .Where("user_id", userId)
                          .GetAsync<DeckEntity>();

            return deck.ToList();
        }

        public async Task<List<DeckSlotEntity>> GetDeckSlotsAsync(int deckId, IDbTransaction? tx = null)
        {
            var deckSlots = await _db.Query("deck_slots")
                                     .Where("deck_id", deckId)
                                     .OrderBy("slot_order")
                                     .GetAsync<DeckSlotEntity>();

            return deckSlots.ToList();
        }

        public async Task<List<DeckSlotEntity>> GetDeckSlotsByDeckIdsAsync(List<int> deckIds, IDbTransaction? tx = null)
        {
            var deckSlots =  await _db.Query("deck_slots")
                                     .WhereIn("deck_id", deckIds)
                                     .OrderBy("slot_order")
                                     .GetAsync<DeckSlotEntity>();

            return deckSlots.ToList();
        }

        public async Task DeleteDeckSlotsAsync(int deckId, IDbTransaction? tx = null)
        {
            await _db.Query("deck_slots")
                     .Where("deck_id", deckId)
                     .DeleteAsync(tx);
        }

        public async Task InsertDeckSlotAsync(DeckSlotEntity slot, IDbTransaction? tx = null)
        {
            await _db.Query("deck_slots")
                     .InsertAsync(new
                     {
                         slot.deck_id,
                         slot.user_character_id,
                         slot.slot_order
                     }, tx);
        }

        public async Task<int> CreateDeckAsync(DeckEntity deck, IDbTransaction? tx = null)
        {
            int deckId = await _db.Query("decks")
                .InsertGetIdAsync<int>(new
                {
                    deck.user_id,
                    deck.deck_index,
                    deck.name,
                    deck.is_active
                }, tx);

            return deckId;
        }

        public async Task CreateDeckSlotAsync(DeckSlotEntity deckSlot, IDbTransaction? tx = null)
        {
            await _db.Query("deck_slots")
                .InsertAsync(new
                {
                    deckSlot.deck_id,
                    deckSlot.user_character_id,
                    deckSlot.slot_order
                }, tx);
        }

        public async Task<DeckEntity> GetDeckForUpdateAsync(int userId, int deckIndex, IDbTransaction? tx = null)
        {
            var sql = "SELECT * FROM decks WHERE user_id = @userId AND deck_index = @deckIndex FOR UPDATE";

            return await _db.Connection.QueryFirstOrDefaultAsync<DeckEntity>(
                sql,
                new { userId, deckIndex },
                tx
            );
        }
    }
}
