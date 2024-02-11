using CodeBase.Gameplay.General;
using CodeBase.Gameplay.Player.Signals;
using UnityEngine;
using Zenject;

namespace CodeBase.Gameplay.Player {
    [RequireComponent(typeof(IHealth))]
    public class PlayerDeathObserver: MonoBehaviour {
        private IHealth _health;
        private SignalBus _signalBus;
        private ICancelable[] _cancelableComponents;

        [Inject]
        public void Construct(SignalBus signalBus) {
            _signalBus = signalBus;
            _cancelableComponents = GetComponents<ICancelable>();
        }

        private void Awake() => _health = GetComponent<IHealth>();

        private void OnEnable() => _health.Changed += FireSignal;
        private void OnDisable() => _health.Changed -= FireSignal;

        private void FireSignal() {
            if(_health.HealthPoints <= 0) _signalBus.Fire(new PlayerDiedSignal(gameObject));
            
            foreach (var cancelable in _cancelableComponents) {
                cancelable.Cancel();
            }
        }
    }
}