using SqlKata.Execution;
using WebServerProject.Models.Entities.Gacha;

namespace WebServerProject.CSR.Repositories
{
    public interface IGachaRepository
    {
        public Task<List<GachaMaster>> GetGachaListAsync();
        public Task<GachaMaster> GetGachaAsync(string gachaCode);
    }
    public class GachaRepository : IGachaRepository
    {
        public readonly QueryFactory _db;
        public GachaRepository(QueryFactory db)
        {
            _db = db;
        }

        public async Task<List<GachaMaster>> GetGachaListAsync()
        {
            var result = await _db.Query("gacha_masters")
                                  .GetAsync<GachaMaster>();

            return result.ToList();
        }

        public async Task<GachaMaster> GetGachaAsync(string gachaCode)
        {
            var result = await _db.Query("gacha_rarity_rates")
                                    .Where("gacha_code", gachaCode)
                                    .FirstOrDefaultAsync<GachaMaster>();

            return result;
        }
    }
}
