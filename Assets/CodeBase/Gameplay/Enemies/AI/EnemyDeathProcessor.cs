using CodeBase.Gameplay.Enemies.Signals;
using CodeBase.Gameplay.General;
using UnityEngine;
using Zenject;

namespace CodeBase.Gameplay.Enemies.AI {
    [RequireComponent(typeof(DeathObserver))]
    public class EnemyDeathProcessor: MonoBehaviour {
        private DeathObserver _deathObserver;
        private SignalBus _signalBus;
        private ICancelable[] _cancelableComponents;

        [Inject]
        public void Construct(SignalBus signalBus) {
            _signalBus = signalBus;
        }

        private void Awake() {
            _deathObserver = GetComponent<DeathObserver>();
            _cancelableComponents = GetComponents<ICancelable>();
        }

        private void OnEnable() => _deathObserver.Died += OnDeath;
        private void OnDisable() => _deathObserver.Died -= OnDeath;

        private void OnDeath() {
            _signalBus.Fire(new EnemyDiedSignal(gameObject));
            
            foreach (var cancelable in _cancelableComponents) {
                cancelable.Cancel();
            }
            Destroy(gameObject);
        }
    }
}