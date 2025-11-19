namespace WebServerProject.Models.Response.Battle
{
    public class StartStageBattleResponse
    {
        public bool success { get; set; }
        public string message { get; set; }
        public int rewardGold { get; set; }
        public int rewardExp { get; set; }
    }
}
