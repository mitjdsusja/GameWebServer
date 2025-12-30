using SqlKata.Execution;
using System;
using System.Data;
using WebServerProject.CSR.Repositories.Gacha;
using WebServerProject.Models.Entities.GachaEntity;

namespace WebServerProject.CSR.Services.Gacha
{
    public interface IGachaRandomizer
    {
        public Task<GachaPool?> SelectItemAsync(string gachaId, QueryFactory? db = null, IDbTransaction? tx = null);
    }

    public class GachaRandomizer : IGachaRandomizer
    {
        public readonly Random _random;

        public ILogger<GachaRandomizer> _logger;
        public readonly IGachaRepository _gachaRepository;

        public GachaRandomizer(
            ILogger<GachaRandomizer> logger,
            IGachaRepository gachaRepository)
        {
            _random = new Random();

            _logger = logger;
            _gachaRepository = gachaRepository;
        }

        public async Task<GachaPool?> SelectItemAsync(string gachaCode, QueryFactory? db = null, IDbTransaction? tx = null)
        {
            // 가챠 정보 불러오기
            var gacha = await _gachaRepository.GetGachaAsync(gachaCode, db, tx);
            if(gacha == null)
            {
                return null;
            }

            // 가챠 확률 불러오기
            var rarityRates = await _gachaRepository.GetGachaRarityRateListAsync(gacha.id, db, tx);
            if(rarityRates == null || rarityRates.Count == 0)
            {
                return null;
            }
            rarityRates = rarityRates.OrderBy(r => r.rarity).ToList();

            // 각 희귀도 확률 합산
            var totalRate = rarityRates.Sum(r => r.rate);
            if (totalRate <= 0)
            {
                return null;
            }

            // 난수 기반 희귀도 결정
            // TODO : Random() 객체 매번 생성x 
            // 트래픽 몰릴시 같은 시드값이 사용될 가능성이 있음. 
            double roll = _random.NextDouble() * totalRate;
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
                return null;
            }

            // 랜덤 아이템 선택
            var selectedItem = poolItems[_random.Next(poolItems.Count)];

            // 반환 

            return selectedItem;
        }
    }
}
