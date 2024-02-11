using CodeBase.Gameplay.ProgressiveValues;

namespace CodeBase.Configs {
    public interface IProgressConfig {
        int GetMoneyForKill(int level);
        int GetSkillPointsForRun(int levelsPassed, int allSkillPoints);
    }
}