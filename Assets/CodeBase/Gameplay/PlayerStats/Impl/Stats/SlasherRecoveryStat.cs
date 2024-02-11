using CodeBase.Gameplay.General.Brains.Impl;
using CodeBase.Gameplay.Player;
using UnityEngine;

namespace CodeBase.Gameplay.PlayerStats.Impl.Stats {
    [CreateAssetMenu(fileName = nameof(SlasherRecoveryStat), menuName = "Player Stats/"+nameof(SlasherRecoveryStat))]
    public class SlasherRecoveryStat : AStatObject {
        public override void Apply(GameObject playerObject) {
            //var slashRecovery = playerObject.GetComponent<SlashRecovery>();
            var slashRecovery = playerObject.GetComponent<NewSlasher>();
            slashRecovery.Recovery.Decrease(ResultMultiplier);
        }
    }
}