using CodeBase.Gameplay.Player;
using UnityEngine;

namespace CodeBase.Gameplay.PlayerStats.Impl.Stats {
    [CreateAssetMenu(fileName = nameof(StaminaRegenStat), menuName = "Player Stats/"+nameof(StaminaRegenStat))]
    public class StaminaRegenStat : AStatObject {
        public override void Apply(GameObject playerObject) {
            var staminaRegen = playerObject.GetComponent<StaminaAutoRegen>();
            staminaRegen.StaminaPerSecond.Increase(ResultMultiplier);
        }
    }
}