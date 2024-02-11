using CodeBase.Gameplay.General.Brains.Impl;
using CodeBase.Gameplay.Player;
using UnityEngine;

namespace CodeBase.Gameplay.PlayerStats.Impl.Stats {
    [CreateAssetMenu(fileName = nameof(SlasherStaminaPriceStat), menuName = "Player Stats/"+nameof(SlasherStaminaPriceStat))]
    public class SlasherStaminaPriceStat : AStatObject {
        public override void Apply(GameObject playerObject) {
            //var slashCharger = playerObject.GetComponent<SlashCharger>();
            var slashCharger = playerObject.GetComponent<NewSlasher>();
            slashCharger.StaminaPrice.Decrease(ResultMultiplier);
        }
    }
}