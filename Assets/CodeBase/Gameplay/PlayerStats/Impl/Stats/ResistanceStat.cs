using CodeBase.Gameplay.General.Impl;
using UnityEngine;

namespace CodeBase.Gameplay.PlayerStats.Impl.Stats {
    [CreateAssetMenu(fileName = nameof(ResistanceStat), menuName = "Player Stats/"+nameof(ResistanceStat))]
    public class ResistanceStat : AStatObject {
        public override void Apply(GameObject playerObject) {
            var hurtProcessor = playerObject.GetComponent<HurtProcessor>();
            hurtProcessor.resistance = ResultMultiplier;
        }
    }
}