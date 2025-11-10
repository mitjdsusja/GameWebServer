using SqlKata.Execution;
using WebServerProject.Models.Entities.Gacha;

namespace WebServerProject.CSR.Repositories
{
    public interface IGachaRepository
    {
        public Task<List<GachaMaster>> GetGachaListAsync();
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
    }
}
