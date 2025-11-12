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

            // 계산

            // 반환 

            throw new NotImplementedException();
        }
    }
}
