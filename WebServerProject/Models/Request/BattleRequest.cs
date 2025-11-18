
namespace WebServerProject.Models.Request
{
    public class StartStageBattleRequest
    {
        public int userId { get; set; }
        public int stageId { get; set; }
        public int deckId { get; set; }
    }
}
