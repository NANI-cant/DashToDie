using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CodeBase.Gameplay.General.Impl {
    [RequireComponent(typeof(IHealth))]
    public class HurtProcessor: MonoBehaviour, IHurtable {
        [SerializeField] public float _resistance = 0;
        [SerializeField] private UnityEvent _onHurt;

        private readonly List<IHurtModifier> _modifiers = new ();
        
        private IHealth _health;

        private void Awake() {
            _health = GetComponent<IHealth>();
        }

        public void TakeHit(int damage, out bool isStillAlive) {
            damage -= Mathf.Min(damage, (int) (damage * _resistance));
            for (int i = 0; i < _modifiers.Count; i++) {
                damage = _modifiers[i].ProcessDamage(damage, this);
            }

            if(damage > 0) _onHurt?.Invoke();
            _health.DecreaseHealth(damage);
            isStillAlive = _health.HealthPoints > 0;
        }

        public void AddModifier(IHurtModifier modifier) => _modifiers.Add(modifier);
        public void RemoveModifier(IHurtModifier modifier) => _modifiers.Remove(modifier);
    }
}