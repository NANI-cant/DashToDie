using CodeBase.Gameplay.General.Impl;
using UnityEngine;
using UnityEngine.Events;

namespace CodeBase.Gameplay.Environment.Collectables.Boosters {
    public class ShieldBooster : MonoBehaviour, IBooster {
        [SerializeField] private UnityEvent<GameObject, ShieldModifier> _onApply;

        public void Apply(GameObject target) {
            var hurtProcessor = target.GetComponent<HurtProcessor>();
            var shield = new ShieldModifier();
            hurtProcessor.AddModifier(shield);
            _onApply?.Invoke(target, shield);
        }
    }
}