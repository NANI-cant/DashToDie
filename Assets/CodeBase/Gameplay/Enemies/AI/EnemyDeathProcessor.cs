using CodeBase.Gameplay.Enemies.Signals;
using CodeBase.Gameplay.General;
using CodeBase.Gameplay.General.Brains;
using CodeBase.Gameplay.VFX;
using UnityEngine;
using Zenject;

namespace CodeBase.Gameplay.Enemies.AI {
    [RequireComponent(typeof(DeathObserver))]
    [RequireComponent(typeof(IBrain))]
    [DisallowMultipleComponent]
    public class EnemyDeathProcessor: MonoBehaviour {
        [SerializeField] private Dissolver _dissolver;
        
        private DeathObserver _deathObserver;
        private IBrain _brain;
        private SignalBus _signalBus;
        private ICancelable[] _cancelableComponents;

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
            _signalBus.Fire(new EnemyDiedSignal(gameObject));
            _brain.Disable();
            foreach (var cancelable in _cancelableComponents) {
                cancelable.Cancel();
            }

            if (_dissolver == null) {
                Destroy(gameObject);    
            }
            else {
                Destroy(gameObject, _dissolver.Duration);
                _dissolver.Dissolve();
            }
            
        }
    }
}