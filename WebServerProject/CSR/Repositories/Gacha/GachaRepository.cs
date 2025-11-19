using SqlKata.Execution;
using WebServerProject.Models.Entities.Gacha;

namespace WebServerProject.CSR.Repositories.Gacha
{
    public interface IGachaRepository
    {
        public Task<List<GachaMaster>> GetGachaListAsync();
        public Task<GachaMaster> GetGachaAsync(string gachaCode);
        public Task<List<GachaRarityRate>> GetGachaRarityRateListAsync(int gachaId);
        public Task<List<GachaPool>> GetGachaPoolByRarityAsync(int gachaId, int rarity);
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
        public Task<GachaMaster> GetGachaAsync(string gachaCode)
        {
            var result =  _db.Query("gacha_masters")
                            .Where("code", gachaCode)
                            .FirstOrDefaultAsync<GachaMaster>();

            return result;
        }

        public async Task<List<GachaRarityRate>> GetGachaRarityRateListAsync(int gachaId)
        {
            var result = await _db.Query("gacha_rarity_rates")
                            .Where("id", gachaId)
                            .GetAsync<GachaRarityRate>();

            return result.ToList();
        }

        public async Task<List<GachaPool>> GetGachaPoolByRarityAsync(int gachaId, int rarity)
        {
            var result = await _db.Query("gacha_pools")
                            .Where("gacha_id", gachaId)
                            .Where("rarity", rarity)
                            .GetAsync<GachaPool>();

            return result.ToList();
        }
    }
}
