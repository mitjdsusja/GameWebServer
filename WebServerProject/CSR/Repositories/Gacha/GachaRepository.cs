using SqlKata.Execution;
using System.Data;
using WebServerProject.Models.Entities.GachaEntity;

namespace WebServerProject.CSR.Repositories.Gacha
{
    public interface IGachaRepository
    {
        public Task<List<GachaMaster>> GetGachaListAsync();
        public Task<GachaMaster> GetGachaAsync(string gachaCode, QueryFactory? db = null, IDbTransaction? tx = null);
        public Task<List<GachaRarityRate>> GetGachaRarityRateListAsync(int gachaId, QueryFactory? db = null, IDbTransaction? tx = null);
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
        public Task<GachaMaster> GetGachaAsync(string gachaCode, QueryFactory? db = null, IDbTransaction? tx = null)
        {
            var q = db ?? _db;

            var result =  q.Query("gacha_masters")
                            .Where("code", gachaCode)
                            .FirstOrDefaultAsync<GachaMaster>(tx);

            return result;
        }

        public async Task<List<GachaRarityRate>> GetGachaRarityRateListAsync(int gachaId, QueryFactory? db = null, IDbTransaction? tx = null)
        {
            var q = db ?? _db;

            var result = await q.Query("gacha_rarity_rates")
                            .Where("gacha_id", gachaId)
                            .GetAsync<GachaRarityRate>(tx);

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
