using WebServerProject.CSR.Repositories;
using WebServerProject.Models.Entities.Gacha;

namespace WebServerProject.CSR.Services
{
    public interface IGachaRandomizer
    {
        public Task<GachaPool> SelectItemAsync(string gachaId);
    }

    public class GachaRandomizer : IGachaRandomizer
    {
        public readonly IGachaRepository gachaRepository;

        public GachaRandomizer(IGachaRepository gachaRepository)
        {
            this.gachaRepository = gachaRepository;
        }

        public async Task<GachaPool> SelectItemAsync(string gachaId)
        {
            // 가챠 정보 불러오기

            // 계산

            // 반환 

            throw new NotImplementedException();
        }
    }
}
