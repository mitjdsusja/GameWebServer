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
        public readonly IGachaRepository _gachaRepository;

        public GachaRandomizer(IGachaRepository gachaRepository)
        {
            _gachaRepository = gachaRepository;
        }

        public async Task<GachaPool> SelectItemAsync(string gachaCode)
        {
            // 가챠 정보 불러오기
            var gacha = await _gachaRepository.GetGachaListAsync();
            var gachaRarityRates = await _gachaRepository.GetGachaAsync(gachaCode);

            if(gacha == null || gachaRarityRates == null)
            {
                throw new Exception("가챠 정보를 불러오는 데 실패했습니다.");
            }
            // 계산

            // 반환 

            throw new NotImplementedException();
        }
    }
}
