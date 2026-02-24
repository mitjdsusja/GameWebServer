using Dapper;
using SqlKata.Execution;
using System.Data;
using WebServerProject.Models.Entities.Gacha;
using WebServerProject.Models.Entities.GachaEntity;

namespace WebServerProject.CSR.Repositories.Gacha
{
    public interface IGachaRepository
    {
        public Task<List<GachaMaster>> GetGachaListAsync();
        public Task<GachaMaster> GetGachaAsync(string gachaCode, QueryFactory? db = null, IDbTransaction? tx = null);
        public Task<List<GachaRarityRate>> GetGachaRarityRateListAsync(int gachaId, QueryFactory? db = null, IDbTransaction? tx = null);
        public Task<List<GachaPool>> GetGachaPoolByRarityAsync(int gachaId, int rarity);
        public Task<UserGachaPity> GetUserGachaPityStackAsync(int userId, int gachaId, QueryFactory? db = null, IDbTransaction? tx = null);
        public Task<UserGachaPity> GetUserGachaPityStackForUpdateAsync(int userId, int gachaId, QueryFactory? db = null, IDbTransaction? tx = null);
        public Task UpsertUserGachaPityStackAsync(int userId, int gachaId, int newPityStack, QueryFactory? db = null, IDbTransaction? tx = null);
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

        public async Task<UserGachaPity> GetUserGachaPityStackAsync(int userId, int gachaId, QueryFactory? db = null, IDbTransaction? tx = null)
        {
            var q = db ?? _db;

            var result = await q.Query("user_gacha_pity")
                            .Where("user_id", userId)
                            .Where("gacha_id", gachaId)
                            .FirstOrDefaultAsync<UserGachaPity>(tx);

            return result;
        }

        public async Task UpsertUserGachaPityStackAsync(int userId, int gachaId, int newPityStack, QueryFactory? db = null, IDbTransaction? tx = null)
        {
            var q = db ?? _db;

            // 트랜잭션 내에서 현재 데이터가 존재하는지 확인 (이미 FOR UPDATE 락이 걸려있어 안전함)
            bool exists = await q.Query("user_gacha_pities")
                .Where("user_id", userId)
                .Where("gacha_id", gachaId)
                .ExistsAsync(tx);

            if (exists)
            {
                // 기존 데이터가 있으면 Update 실행
                await q.Query("user_gacha_pities")
                    .Where("user_id", userId)
                    .Where("gacha_id", gachaId)
                    .UpdateAsync(new
                    {
                        pity_stack = newPityStack,
                        updated_at = DateTime.UtcNow // 갱신 시간을 현재 서버 시간으로 업데이트
                    }, tx);
            }
            else
            {
                // 첫 뽑기라서 데이터가 없으면 Insert 실행
                await q.Query("user_gacha_pities")
                    .InsertAsync(new
                    {
                        user_id = userId,
                        gacha_id = gachaId,
                        pity_stack = newPityStack
                        // updated_at은 테이블의 DEFAULT 조건에 의해 DB 시간이 자동으로 들어감
                    }, tx);
            }
        }

        public async Task<UserGachaPity> GetUserGachaPityStackForUpdateAsync(int userId, int gachaId, QueryFactory? db = null, IDbTransaction? tx = null)
        {
            var q = db ?? _db;

            string sql = "SELECT * FROM user_gacha_pities WHERE user_id = @UserId AND gacha_id = @GachaId FOR UPDATE";

            return await q.Connection.QueryFirstOrDefaultAsync<UserGachaPity>(
                sql,
                new { UserId = userId, GachaId = gachaId },
                tx
            );
        }
    }
}
