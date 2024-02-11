using CodeBase.Gameplay.Skills;
using CodeBase.Gameplay.Skills.Impl;

namespace CodeBase.Gameplay.Environment.Collectables.Boosters {
    public class HyperArmorChargeBooster : SkillChargeBooster {
        protected override ISkill GetTargetSkill(ASkillHolder skillHolder)
            => skillHolder.GetSkill<HyperArmorSkill>();
    }
}