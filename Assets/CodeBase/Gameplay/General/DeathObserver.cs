using System;
using UnityEngine;

namespace CodeBase.Gameplay.General {
    [RequireComponent(typeof(IHealth))]
    [DisallowMultipleComponent]
    public sealed class DeathObserver : MonoBehaviour {
        private IHealth _health;

        public event Action Died;
        
        private void Awake() => _health = GetComponent<IHealth>();

        private void OnEnable() => _health.Changed += Notify;
        private void OnDisable() => _health.Changed -= Notify;
        
        private void Notify() {
            if (_health.HealthPoints <= 0) Died?.Invoke();
        }
    }
}