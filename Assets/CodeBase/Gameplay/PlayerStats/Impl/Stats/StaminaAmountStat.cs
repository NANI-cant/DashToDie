using CodeBase.Gameplay.Player;
using UnityEngine;

namespace CodeBase.Gameplay.PlayerStats.Impl.Stats {
    [CreateAssetMenu(fileName = nameof(StaminaAmountStat), menuName = "Player Stats/"+nameof(StaminaAmountStat))]
    public class StaminaAmountStat : AStatObject {
        public override void Apply(GameObject playerObject) {
            var stamina = playerObject.GetComponent<Stamina>();
            stamina.Capacity.Increase(ResultMultiplier);
        }
    }
}