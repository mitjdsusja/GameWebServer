using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace ProjectTest
{
    public class GachaProbabilityTests
    {
        private readonly ITestOutputHelper _output;

        public GachaProbabilityTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Detailed_Gacha_Simulation_Log()
        {
            // 1. 설정
            int totalUsers = 100; // 너무 많으면 로그 보기 힘드니 우선 100명만 상세히 볼게요.
            int softPityThreshold = 60;
            int hardPityThreshold = 80;
            double baseRate = 1.0;
            double bonusRatePerStack = 2.0;

            _output.WriteLine("=== 가챠 당첨 상세 리포트 ===");
            _output.WriteLine("유저번호 | 당첨회차 | 당첨직전확률 | 비고");
            _output.WriteLine("--------------------------------------------");

            for (int i = 1; i <= totalUsers; i++)
            {
                int currentStack = 0;
                bool isWon = false;

                while (!isWon)
                {
                    currentStack++;
                    double currentProbability = baseRate;

                    // 확률 보정 로직 (실제 서비스 로직과 동일)
                    if (currentStack >= hardPityThreshold)
                    {
                        currentProbability = 100.0;
                    }
                    else if (currentStack >= softPityThreshold)
                    {
                        int bonusStacks = currentStack - softPityThreshold + 1;
                        currentProbability += (bonusStacks * bonusRatePerStack);
                    }

                    // 추첨
                    double roll = Random.Shared.NextDouble() * 100.0;
                    if (roll <= currentProbability)
                    {
                        isWon = true;

                        // 로그 출력
                        string note = currentStack >= hardPityThreshold ? "천장당첨!!" :
                                     (currentStack >= softPityThreshold ? "소프트피티" : "일반당첨");

                        _output.WriteLine($"{i:D3}번 유저 | {currentStack:D2}회차 | {currentProbability:F1}% | {note}");
                    }
                }
            }
        }

        [Fact]
        public void Gacha_Simulation_Final_Report()
        {
            // 1. 설정
            int totalUsers = 100000;
            int softPityThreshold = 60;
            int hardPityThreshold = 80;
            double baseRate = 1.0;
            double bonusRatePerStack = 2.0;

            long totalPullsAcrossAllUsers = 0; // 전체 유저가 사용한 총 티켓 수
            List<int> results = new List<int>();
            int hardPityWins = 0;
            int softPityWins = 0;

            for (int i = 0; i < totalUsers; i++)
            {
                int currentStack = 0;
                bool isWon = false;

                while (!isWon)
                {
                    currentStack++;
                    totalPullsAcrossAllUsers++; // 시도 횟수 누적

                    double currentProbability = baseRate;
                    if (currentStack >= hardPityThreshold) currentProbability = 100.0;
                    else if (currentStack >= softPityThreshold)
                    {
                        currentProbability += (currentStack - softPityThreshold + 1) * bonusRatePerStack;
                    }

                    if (Random.Shared.NextDouble() * 100.0 <= currentProbability)
                    {
                        isWon = true;
                        results.Add(currentStack);
                        if (currentStack == hardPityThreshold) hardPityWins++;
                        else if (currentStack >= softPityThreshold) softPityWins++;
                    }
                }
            }

            // 2. 결과 분석 (팩트 체크)
            // 실효 확률 = (당첨 횟수 / 전체 시도 횟수) * 100
            double effectiveRate = ((double)totalUsers / totalPullsAcrossAllUsers) * 100.0;

            _output.WriteLine("=== 📊 가챠 확률 최종 정밀 리포트 ===");
            _output.WriteLine($"총 테스트 인원   : {totalUsers:N0}명");
            _output.WriteLine($"총 사용된 티켓   : {totalPullsAcrossAllUsers:N0}장");
            _output.WriteLine("--------------------------------------------");
            _output.WriteLine($"표기 확률 (Base) : {baseRate:F2}%");
            _output.WriteLine($"실효 확률 (Eff.) : {effectiveRate:F2}%  <-- 💡 핵심 수치");
            _output.WriteLine("--------------------------------------------");
            _output.WriteLine($"평균 당첨 회차   : {results.Average():F2}회");
            _output.WriteLine($"하드 천장 비율   : {((double)hardPityWins / totalUsers) * 100:F2}%");
            _output.WriteLine("--------------------------------------------");
        }
    }
}
