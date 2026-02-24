using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Moq;
using WebServerProject.CSR.Repositories.Gacha;
using WebServerProject.CSR.Services.Gacha;
using WebServerProject.Models.Entities.GachaEntity;
using Xunit.Abstractions;

namespace ProjectTest
{
    public class GachaRandomizerTests
    {
        private readonly ITestOutputHelper _output;

        // 생성자를 통해 xUnit의 출력 헬퍼를 주입받습니다.
        public GachaRandomizerTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task SelectItemAsync_HardPity_ShouldReturnTargetRarityItem()
        {
            // 1. 의존성 Mock 생성
            // GachaRandomizer는 생성자에서 Logger와 Repository만 받으므로 2개만 준비합니다.
            var mockLogger = new Mock<ILogger<GachaRandomizer>>();
            var mockRepo = new Mock<IGachaRepository>();

            // 2. 가짜 데이터 설정
            var gacha = new GachaMaster
            {
                id = 1,
                soft_pity_threshold = 60,
                hard_pity_threshold = 80,
                pity_bonus_rate = 2.0,
                pity_target_rarity = 3
            };

            // DB에서 가져올 등급 확률 (평소에는 4등급이 1%라고 가정)
            var baseRates = new List<GachaRarityRate>
            {
                new GachaRarityRate { rarity = 4, rate = 1.0 },
                new GachaRarityRate { rarity = 3, rate = 9.0 },
                new GachaRarityRate { rarity = 2, rate = 20.0 },
                new GachaRarityRate { rarity = 1, rate = 70.0 }
            };

            mockRepo.Setup(r => r.GetGachaRarityRateListAsync(It.IsAny<int>(), null, null))
                    .ReturnsAsync(baseRates);

            // 당첨 시 반환될 전설 아이템 풀
            var mockPool = new List<GachaPool>
            {
                new GachaPool { id = 999, rarity = 3 }
            };

            mockRepo.Setup(r => r.GetGachaPoolByRarityAsync(It.IsAny<int>(), 3))
                    .ReturnsAsync(mockPool);

            // 3. 테스트 대상 객체(GachaRandomizer) 생성
            var randomizer = new GachaRandomizer(mockLogger.Object, mockRepo.Object);

            // 4. 실행: 79스택 상태 (다음은 80회차 = 하드 천장 발동)
            var result = await randomizer.SelectItemAsync(gacha, 79);

            // 5. 검증
            Assert.NotNull(result);
            Assert.Equal(3, result.rarity); // 천장이 작동했다면 반드시 3등급이어야 함
            Assert.Equal(999, result.id);
        }

        [Fact]
        public async Task SelectItemAsync_100k_Simulation_Report()
        {
            // 1. 준비 (Mock 설정)
            var mockLogger = new Mock<ILogger<GachaRandomizer>>();
            var mockRepo = new Mock<IGachaRepository>();

            var gacha = new GachaMaster
            {
                id = 1,
                soft_pity_threshold = 60,
                hard_pity_threshold = 80,
                pity_bonus_rate = 2.0,
                pity_target_rarity = 4
            };

            var baseRates = new List<GachaRarityRate> 
            {
                new GachaRarityRate { rarity = 4, rate = 1.0 },
                new GachaRarityRate { rarity = 3, rate = 9.0 },
                new GachaRarityRate { rarity = 2, rate = 20.0 },
                new GachaRarityRate { rarity = 1, rate = 70.0 }
            };

            mockRepo.Setup(r => r.GetGachaRarityRateListAsync(It.IsAny<int>(), null, null))
                    .ReturnsAsync(baseRates);

            var mockPool = new List<GachaPool> { new GachaPool { id = 999, rarity = 4 } };
            mockRepo.Setup(r => r.GetGachaPoolByRarityAsync(It.IsAny<int>(), 4))
                    .ReturnsAsync(mockPool);

            var randomizer = new GachaRandomizer(mockLogger.Object, mockRepo.Object);

            // 2. 시뮬레이션 변수
            int totalUsers = 100000;
            long totalPulls = 0;
            int hardPityCount = 0;
            List<int> pullHistory = new List<int>();

            // 3. 실행 (10만 명의 가상 유저 시뮬레이션)
            for (int i = 0; i < totalUsers; i++)
            {
                int currentStack = 0;
                bool isWon = false;

                while (!isWon)
                {
                    var result = await randomizer.SelectItemAsync(gacha, currentStack);
                    currentStack++;
                    totalPulls++;

                    if (result != null && result.rarity == 4) // 당첨!
                    {
                        isWon = true;
                        pullHistory.Add(currentStack);
                        if (currentStack == 80) hardPityCount++;
                    }
                }
            }

            // 4. 결과 분석 및 출력
            double effectiveRate = ((double)totalUsers / totalPulls) * 100.0;
            double averagePulls = pullHistory.Average();

            _output.WriteLine($"=== 📊 GachaRandomizer 최종 검증 리포트 ===");
            _output.WriteLine($"총 시뮬레이션 인원 : {totalUsers:N0}명");
            _output.WriteLine($"총 소모된 티켓 수   : {totalPulls:N0}장");
            _output.WriteLine($"표기 확률 (Base)    : 1.00%");
            _output.WriteLine($"실효 확률 (Effective): {effectiveRate:F2}%");
            _output.WriteLine($"평균 당첨 회차      : {averagePulls:F2}회");
            _output.WriteLine($"하드 천장 도달 유저 : {hardPityCount}명 ({((double)hardPityCount / totalUsers) * 100:F2}%)");
        }
    }
}
