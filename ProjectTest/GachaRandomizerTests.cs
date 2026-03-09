using Microsoft.Extensions.Logging;
using Moq;
using System.Data;
using WebServerProject.CSR.Repositories.Gacha;
using WebServerProject.CSR.Services.Gacha;
using WebServerProject.Models.Entities.GachaEntity;
using Xunit;
using Xunit.Abstractions;

namespace ProjectTest
{
    public class GachaRandomizerTests
    {
        private readonly ITestOutputHelper _output;

        public GachaRandomizerTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task SelectItemAsync_HardPity_ShouldReturnTargetRarityItem()
        {
            // 1. Mock 생성
            var mockLogger = new Mock<ILogger<GachaRandomizer>>();
            var mockRepo = new Mock<IGachaRepository>();

            // 2. 가짜 데이터 (GachaMaster)
            var gacha = new GachaMaster
            {
                id = 1,
                code = "test_gacha",
                soft_pity_threshold = 60,
                hard_pity_threshold = 80,
                pity_bonus_rate = 2.0,
                pity_target_rarity = 4 // 4성이 천장 타겟
            };

            // 확률표 Mock
            var baseRates = new List<GachaRarityRate>
            {
                new GachaRarityRate { rarity = 4, rate = 1.0 },
                new GachaRarityRate { rarity = 3, rate = 9.0 },
                new GachaRarityRate { rarity = 2, rate = 20.0 },
                new GachaRarityRate { rarity = 1, rate = 70.0 }
            };

            // [수정 포인트] 파라미터 매칭을 It.IsAny로 유연하게 설정 (null, db, tx 문제 해결)
            mockRepo.Setup(r => r.GetGachaRarityRateListAsync(It.IsAny<int>(), It.IsAny<IDbTransaction>()))
                    .ReturnsAsync(baseRates);

            // 아이템 풀 Mock (3성 아이템 반환)
            var mockPool = new List<GachaPool> { new GachaPool { id = 999, rarity = 3 } };

            // [수정 포인트] tx 파라미터 매칭 추가
            mockRepo.Setup(r => r.GetGachaPoolByRarityAsync(It.IsAny<int>(), 3, It.IsAny<IDbTransaction>()))
                    .ReturnsAsync(mockPool);

            // 3. 객체 생성
            var randomizer = new GachaRandomizer(mockLogger.Object, mockRepo.Object);

            // 4. 실행 (79스택 -> 다음 80번째는 하드 천장)
            var result = await randomizer.SelectItemAsync(gacha.code, 79);

            // 5. 검증
            Assert.NotNull(result);
            Assert.Equal(4, result.rarity); // 천장 타겟(4)이 나와야 함
            Assert.Equal(999, result.id);
        }

        [Fact]
        public async Task SelectItemAsync_100k_Simulation_Report()
        {
            // 1. Mock 생성
            var mockLogger = new Mock<ILogger<GachaRandomizer>>();
            var mockRepo = new Mock<IGachaRepository>();

            var gacha = new GachaMaster
            {
                id = 1,
                code = "test_gacha", // 코드 확인
                soft_pity_threshold = 60,
                hard_pity_threshold = 80,
                pity_bonus_rate = 2.0,
                pity_target_rarity = 4
            };

            // [핵심 수정 포인트] GetGachaAsync에 대한 Mock 추가!
            // 이게 없어서 그동안 내부에서 gacha가 null이라 아무것도 실행 안 됨
            mockRepo.Setup(r => r.GetGachaAsync(It.IsAny<string>(), It.IsAny<IDbTransaction>()))
                    .ReturnsAsync(gacha);

            var baseRates = new List<GachaRarityRate>
            {
                new GachaRarityRate { rarity = 4, rate = 1.0 },
                new GachaRarityRate { rarity = 3, rate = 9.0 },
                new GachaRarityRate { rarity = 2, rate = 20.0 },
                new GachaRarityRate { rarity = 1, rate = 70.0 }
            };

            // 확률표 Mock
            mockRepo.Setup(r => r.GetGachaRarityRateListAsync(It.IsAny<int>(), It.IsAny<IDbTransaction>()))
                    .ReturnsAsync(baseRates);

            // 아이템 풀 Mock (모든 등급 대응)
            mockRepo.Setup(r => r.GetGachaPoolByRarityAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<IDbTransaction>()))
                    .ReturnsAsync((int id, int rarity, IDbTransaction tx) =>
                    {
                        return new List<GachaPool> { new GachaPool { id = rarity * 100, rarity = rarity } };
                    });

            var randomizer = new GachaRandomizer(mockLogger.Object, mockRepo.Object);

            // 2. 시뮬레이션
            int totalUsers = 100000;
            long totalPulls = 0;
            int hardPityCount = 0;
            List<int> pullHistory = new List<int>();

            // 3. 실행
            for (int i = 0; i < totalUsers; i++)
            {
                int currentStack = 0;
                bool isWon = false;

                while (!isWon)
                {
                    // 이제 GetGachaAsync가 정상 동작하므로 결과가 나옴
                    var result = await randomizer.SelectItemAsync(gacha.code, currentStack);

                    currentStack++;
                    totalPulls++;

                    if (result != null && result.rarity == 4)
                    {
                        isWon = true;
                        pullHistory.Add(currentStack);
                        if (currentStack == 80) hardPityCount++;
                    }

                    // 무한 루프 방지
                    if (currentStack > 200) break;
                }
            }

            // 4. 안전장치 추가 (혹시라도 리스트가 비었으면 0으로 처리)
            double averagePulls = pullHistory.Count > 0 ? pullHistory.Average() : 0;
            double effectiveRate = totalPulls > 0 ? ((double)totalUsers / totalPulls) * 100.0 : 0;

            _output.WriteLine($"=== 📊 GachaRandomizer 최종 검증 리포트 ===");
            _output.WriteLine($"총 시뮬레이션 인원 : {totalUsers:N0}명");
            _output.WriteLine($"총 소모된 티켓 수  : {totalPulls:N0}장");
            _output.WriteLine($"실효 확률 (Effective): {effectiveRate:F2}%");
            _output.WriteLine($"평균 당첨 회차      : {averagePulls:F2}회");
            _output.WriteLine($"하드 천장 도달 유저 : {hardPityCount:N0}명 ({((double)hardPityCount / totalUsers) * 100:F2}%)");

            // 검증 (최소 한 명은 당첨되어야 함)
            Assert.True(pullHistory.Count > 0, "아무도 당첨되지 않았습니다. 로직을 점검하세요.");
        }
    }
}