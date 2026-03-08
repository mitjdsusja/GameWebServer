using SqlKata.Execution;
using System.Data;
using WebServerProject.Models.Entities.GachaEntity;

namespace WebServerProject.CSR.Repositories.Gacha
{
    public interface IGachaRepository
    {
        public Task<List<GachaMaster>> GetGachaListAsync(IDbTransaction? tx = null);
        public Task<GachaMaster> GetGachaAsync(string gachaCode, IDbTransaction? tx = null);
        public Task<List<GachaRarityRate>> GetGachaRarityRateListAsync(int gachaId, IDbTransaction? tx = null);
        public Task<List<GachaPool>> GetGachaPoolByRarityAsync(int gachaId, int rarity, IDbTransaction? tx = null);
    }
    public class GachaRepository : IGachaRepository
    {
        public readonly QueryFactory _db;
        public GachaRepository(QueryFactory db)
        {
            _db = db;
        }

        public async Task<List<GachaMaster>> GetGachaListAsync(IDbTransaction? tx = null)
        {
            var result = await _db.Query("gacha_masters")
                                  .GetAsync<GachaMaster>(tx);

            return result.ToList();
        }
        public Task<GachaMaster> GetGachaAsync(string gachaCode, IDbTransaction? tx = null)
        {
            var result =  _db.Query("gacha_masters")
                            .Where("code", gachaCode)
                            .FirstOrDefaultAsync<GachaMaster>(tx);

            return result;
        }

        public async Task<List<GachaRarityRate>> GetGachaRarityRateListAsync(int gachaId, IDbTransaction? tx = null)
        {
            var result = await _db.Query("gacha_rarity_rates")
                            .Where("gacha_id", gachaId)
                            .GetAsync<GachaRarityRate>(tx);

            return result.ToList();
        }

        public async Task<List<GachaPool>> GetGachaPoolByRarityAsync(int gachaId, int rarity, IDbTransaction? tx = null)
        {
            var result = await _db.Query("gacha_pools")
                            .Where("gacha_id", gachaId)
                            .Where("rarity", rarity)
                            .GetAsync<GachaPool>(tx);

            return result.ToList();
        }
    }
}
