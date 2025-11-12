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
            var gacha = await _gachaRepository.GetGachaAsync(gachaCode);
            if(gacha == null)
            {
                throw new InvalidOperationException("가챠 정보를 불러오는 데 실패했습니다.");
            }

            // 가챠 확률 불러오기
            var rarityRates = await _gachaRepository.GetGachaRarityRateListAsync(gacha.id);
            if(rarityRates == null || rarityRates.Count == 0)
            {
                throw new InvalidOperationException("가챠 확률 정보를 불러오는 데 실패했습니다.");
            }

            // 각 희귀도 확률 합산
            var totalRate = rarityRates.Sum(r => r.rate);
            if (totalRate <= 0)
            {
                throw new InvalidOperationException($"가챠 [{gachaCode}]의 확률 합계가 0 이하입니다.");
            }

            // 난수 기반 희귀도 결정
            var random = new Random();
            double roll = random.NextDouble() * totalRate;
            double cumulative = 0;
            int selectedRarity = 1;

            foreach (var rate in rarityRates)
            {
                cumulative += rate.rate;
                if (roll <= cumulative)
                {
                    selectedRarity = rate.rarity;
                    break;
                }
            }

            // 해당 희귀도의 아이템 풀 조회
            var poolItems = await _gachaRepository.GetGachaPoolByRarityAsync(gacha.id, selectedRarity);
            if (poolItems == null || poolItems.Count == 0)
            {
                throw new InvalidOperationException($"가챠 [{gachaCode}]의 희귀도 [{selectedRarity}] 풀 데이터가 없습니다.");
            }

            // 랜덤 아이템 선택
            var selectedItem = poolItems[random.Next(poolItems.Count)];

            // 반환 

            return selectedItem;
        }
    }
}
