using CodeBase.Gameplay.General;
using UnityEngine;

namespace CodeBase.Gameplay.PlayerStats.Impl.Stats {
    [CreateAssetMenu(fileName = nameof(DamageStat), menuName = "Player Stats/"+nameof(DamageStat))]
    public class DamageStat : AStatObject {
        public override void Apply(GameObject playerObject) {
            var damageDealer = playerObject.GetComponent<IDamageDealer>();
            damageDealer.Damage.Increase(ResultMultiplier);
        }
    }
}