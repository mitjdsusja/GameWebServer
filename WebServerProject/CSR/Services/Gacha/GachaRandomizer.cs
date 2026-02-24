using SqlKata.Execution;
using System;
using System.Data;
using WebServerProject.CSR.Repositories.Gacha;
using WebServerProject.Models.Entities.GachaEntity;

namespace WebServerProject.CSR.Services.Gacha
{
    public interface IGachaRandomizer
    {
        public Task<GachaPool?> SelectItemAsync(GachaMaster gacha, int pityStack, QueryFactory? db = null, IDbTransaction? tx = null);
    }

    public class GachaRandomizer : IGachaRandomizer
    {
        public ILogger<GachaRandomizer> _logger;
        public readonly IGachaRepository _gachaRepository;

        public GachaRandomizer(
            ILogger<GachaRandomizer> logger,
            IGachaRepository gachaRepository)
        {
            _logger = logger;
            _gachaRepository = gachaRepository;
        }

        public async Task<GachaPool?> SelectItemAsync(GachaMaster gacha, int pityStack, QueryFactory? db = null, IDbTransaction? tx = null)
        {
            // 가챠 확률 불러오기
            List<GachaRarityRate> baseRarityRates = await _gachaRepository.GetGachaRarityRateListAsync(gacha.id , db, tx);
            if (baseRarityRates == null || baseRarityRates.Count == 0)
            {
                return null;
            }

            // 천장 시스템을 적용하여 확률 동적 재계산
            var adjustedRates = ApplyHybridPity(
                baseRarityRates,
                pityStack,
                gacha.soft_pity_threshold,
                gacha.hard_pity_threshold,
                gacha.pity_bonus_rate,
                gacha.pity_target_rarity
            );

            // 재계산된 확률 리스트를 등급순으로 정렬
            adjustedRates = adjustedRates.OrderBy(r => r.rarity).ToList();

            // 각 희귀도 확률 합산
            var totalRate = adjustedRates.Sum(r => r.rate);
            if (totalRate <= 0)
            {
                return null;
            }

            // 0부터 총 확률(totalRate) 사이의 무작위 목표 지점 설정
            double targetProbability = Random.Shared.NextDouble() * totalRate;

            // 확률 누적기 초기화 및 기본 희귀도 설정
            double currentCumulativeProbability = 0.0;
            int selectedRarity = 1; // 기본값

            foreach (var rarityInfo in adjustedRates)
            {
                currentCumulativeProbability += rarityInfo.rate;

                if (targetProbability <= currentCumulativeProbability)
                {
                    selectedRarity = rarityInfo.rarity;
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
            var selectedItem = poolItems[Random.Shared.Next(poolItems.Count)];

            return selectedItem;
        }

        private List<GachaRarityRate>  ApplyHybridPity(
            List<GachaRarityRate> baseRates,
            int currentPityStack,
            int softPityThreshold,
            int hardPityThreshold,
            double bonusRatePerStack,
            int targetRarity)
        {
            var adjustedRates = baseRates.Select(r => new GachaRarityRate { rarity = r.rarity, rate = r.rate }).ToList();
            int nextPullCount = currentPityStack + 1;

            // 하드 피티: 100% 확정
            if (hardPityThreshold > 0 && nextPullCount >= hardPityThreshold)
            {
                foreach (var rateObj in adjustedRates)
                {
                    rateObj.rate = (rateObj.rarity == targetRarity) ? 100.0 : 0.0;
                }
                return adjustedRates;
            }

            // 소프트 피티: 점진적 증가
            if (softPityThreshold > 0 && nextPullCount >= softPityThreshold)
            {
                int bonusStacks = nextPullCount - softPityThreshold + 1;
                double totalBonusRate = bonusStacks * bonusRatePerStack;

                var targetRateObj = adjustedRates.FirstOrDefault(r => r.rarity == targetRarity);
                if (targetRateObj != null)
                {
                    targetRateObj.rate += totalBonusRate;
                }
            }

            return adjustedRates;
        }
    }
}
