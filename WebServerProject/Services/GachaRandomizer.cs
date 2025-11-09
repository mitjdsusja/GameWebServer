using WebServerProject.Models.Entities.Gacha;
using WebServerProject.Repositories;

namespace WebServerProject.Services
{
    public interface IGachaRandomizer
    {
        public Task<GachaPool> SelectItemAsync(int gachaId);
    }

    public class GachaRandomizer : IGachaRandomizer
    {
        public readonly IGachaRepository gachaRepository;

        public GachaRandomizer(IGachaRepository gachaRepository)
        {
            this.gachaRepository = gachaRepository;
        }

        public async Task<GachaPool> SelectItemAsync(int gachaId)
        {
            // 가챠 정보 불러오기

            // 계산

            // 반환 

            throw new NotImplementedException();
        }
    }
}
