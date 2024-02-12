using System.Collections.Generic;
using UnityEngine;

namespace CodeBase.Gameplay.General.Impl {
    [RequireComponent(typeof(IHealth))]
    public class HurtProcessor: MonoBehaviour, IHurtable {
        [SerializeField] public float _resistance = 0;

        private readonly List<IHurtModifier> _modifiers = new ();
        
        private IHealth _health;

        private void Awake() {
            _health = GetComponent<IHealth>();
        }

        public void TakeHit(int damage, out bool isStillAlive) {
            damage -= Mathf.Min(damage, (int) (damage * _resistance));
            foreach (var modifier in _modifiers) {
                damage = modifier.ProcessDamage(damage, this);
            }

            _health.DecreaseHealth(damage);
            isStillAlive = _health.HealthPoints > 0;
        }

        public void AddModifier(IHurtModifier modifier) => _modifiers.Add(modifier);
        public void RemoveModifier(IHurtModifier modifier) => _modifiers.Remove(modifier);
    }
}