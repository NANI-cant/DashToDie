using CodeBase.Gameplay.General;
using CodeBase.Gameplay.General.Brains;
using CodeBase.Gameplay.Player.Signals;
using UnityEngine;
using Zenject;

namespace CodeBase.Gameplay.Player {
    [RequireComponent(typeof(IHealth))]
    public class PlayerDeathProcessor: MonoBehaviour {
        private DeathObserver _deathObserver;
        private SignalBus _signalBus;
        private ICancelable[] _cancelableComponents;
        private IBrain _brain;

        [Inject]
        public void Construct(SignalBus signalBus) {
            _signalBus = signalBus;
        }

        private void Awake() {
            _deathObserver = GetComponent<DeathObserver>();
            _cancelableComponents = GetComponents<ICancelable>();
            _brain = GetComponent<IBrain>();
        }

        private void OnEnable() => _deathObserver.Died += OnDeath;
        private void OnDisable() => _deathObserver.Died -= OnDeath;

        private void OnDeath() {
            _signalBus.Fire(new PlayerDiedSignal(gameObject));
            _brain.Disable();
            foreach (var cancelable in _cancelableComponents) {
                cancelable.Cancel();
            }
        }
    }
}