using CodeBase.Gameplay.Player;
using CodeBase.Gameplay.Skills.Impl;
using UnityEngine;

namespace CodeBase.Gameplay.PlayerStats.Impl.Stats {
    [CreateAssetMenu(fileName = nameof(SlayAllDelayStat), menuName = "Player Stats/"+nameof(SlayAllDelayStat))]
    public class SlayAllDelayStat : AStatObject {
        public override void Apply(GameObject playerObject) {
            var skillHolder = playerObject.GetComponent<PlayerSkillHolder>();
            SlayAllSkill slayAllSkill = skillHolder.GetSkill<SlayAllSkill>();

            slayAllSkill.Delay.Decrease(ResultMultiplier);
        }
    }
}