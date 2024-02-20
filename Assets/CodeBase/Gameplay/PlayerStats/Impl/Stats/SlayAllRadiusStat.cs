using CodeBase.Gameplay.Player;
using CodeBase.Gameplay.Skills.Impl;
using UnityEngine;

namespace CodeBase.Gameplay.PlayerStats.Impl.Stats {
    [CreateAssetMenu(fileName = nameof(SlayAllRadiusStat), menuName = "Player Stats/"+nameof(SlayAllRadiusStat))]
    public class SlayAllRadiusStat : AStatObject {
        public override void Apply(GameObject playerObject) {
            var skillHolder = playerObject.GetComponent<PlayerSkillHolder>();
            SlayAllSkill slayAllSkill = skillHolder.GetSkill<SlayAllSkill>();

            slayAllSkill.Radius.Increase(ResultMultiplier);
        }
    }
}