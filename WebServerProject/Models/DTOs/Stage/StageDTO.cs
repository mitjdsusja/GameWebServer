namespace WebServerProject.Models.DTOs.Stage
{
    public class StageDTO
    {
        public int id { get; set; }
        public int chapter { get; set; }
        public int stageNumber { get; set; }
        public int rewardGold { get; set; }
        public int rewardExp { get; set; }

        public List<StageEnemyDTO> stageEnemies { get; set; }

        public static StageDTO FromStage(Entities.Stage.Stage stage)
        {
            return new StageDTO
            {
                id = stage.id,
                chapter = stage.chapter,
                stageNumber = stage.stage_number,
                rewardGold = stage.reward_gold,
                rewardExp = stage.reward_exp
            };
        }
    }
}
