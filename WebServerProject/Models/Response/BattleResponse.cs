namespace WebServerProject.Models.Response
{
    public class StartStageBattleResponse
    {
        public bool success { get; set; }
        public string message { get; set; }
        public int rewardGold { get; set; }
        public int rewardExp { get; set; }
    }
}
