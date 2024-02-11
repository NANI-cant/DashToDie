using CodeBase.Gameplay.Player;
using CodeBase.Gameplay.Skills;
using UnityEngine;

namespace CodeBase.Gameplay.Environment.Collectables.Boosters {
    public abstract class SkillChargeBooster : MonoBehaviour, IBooster {
        [SerializeField] [Min(1)] private int _charges = 1;

        public void Apply(GameObject target) {
            var targetSkill = GetTargetSkill(target.GetComponent<PlayerSkillHolder>()); 
            targetSkill.ChargeCounter.Charges++;
        }

        protected abstract ISkill GetTargetSkill(ASkillHolder skillHolder);
    }
}