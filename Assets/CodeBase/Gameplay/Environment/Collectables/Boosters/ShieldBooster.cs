using CodeBase.Gameplay.General.Impl;
using UnityEngine;

namespace CodeBase.Gameplay.Environment.Collectables.Boosters {
    public class ShieldBooster : MonoBehaviour, IBooster {
        public void Apply(GameObject target) {
            var hurtProcessor = target.GetComponent<HurtProcessor>();
            hurtProcessor.AddModifier(new ShieldModifier());
        }
    }
}