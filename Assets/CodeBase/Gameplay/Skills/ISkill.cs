using ICancelable = CodeBase.Gameplay.General.ICancelable;

namespace CodeBase.Gameplay.Skills {
    public interface ISkill: ICancelable {
        bool IsExecuting { get; }
        IChargeCounter ChargeCounter { get; }

        bool TryExecute(ASkillHolder holder, int holderTeam);
    }
}