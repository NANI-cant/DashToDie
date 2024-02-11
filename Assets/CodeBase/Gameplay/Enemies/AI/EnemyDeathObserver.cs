using CodeBase.Gameplay.Enemies.Signals;
using CodeBase.Gameplay.General;
using UnityEngine;
using Zenject;

namespace CodeBase.Gameplay.Enemies.AI {
    [RequireComponent(typeof(IHealth))]
    public class EnemyDeathObserver: MonoBehaviour {
        private IHealth _health;
        private SignalBus _signalBus;

        [Inject]
        public void Construct(SignalBus signalBus) {
            _signalBus = signalBus;
        }

        private void Awake() => _health = GetComponent<IHealth>();

        private void OnEnable() => _health.Changed += FireSignal;
        private void OnDisable() => _health.Changed -= FireSignal;

        private void FireSignal() {
            if (_health.HealthPoints <= 0) _signalBus.Fire(new EnemyDiedSignal(gameObject));
        }
    }
}