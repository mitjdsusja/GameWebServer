namespace WebServerProject.Models.DTOs.Battle
{
    public class StartStageBattleResult
    {
        public bool success { get; set; }
        public string message { get; set; }

        public int rewardGold { get; set; }
        public int rewardExp { get; set; }
    }

    public class BattleRewardDTO
    {
        public int userId { get; set; }
        public int gold { get; set; }
        public int exp { get; set; }
    }
}
