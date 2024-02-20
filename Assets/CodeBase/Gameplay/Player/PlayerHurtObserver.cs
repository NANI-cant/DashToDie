using CodeBase.Gameplay.General;
using CodeBase.Gameplay.Player.Signals;
using UnityEngine;
using Zenject;

namespace CodeBase.Gameplay.Player {
    [RequireComponent(typeof(IHealth))]
    [DisallowMultipleComponent]
    public class PlayerHurtObserver : MonoBehaviour {
        private IHealth _targetHealth;
        private SignalBus _signalBus;

        [Inject]
        public void Construct(SignalBus signalBus) {
            _signalBus = signalBus;
        }

        private void Awake() {
            _targetHealth = GetComponent<IHealth>();
        }

        private void OnEnable() => _targetHealth.Decreased += FireSignal;
        private void OnDisable() => _targetHealth.Decreased -= FireSignal;

        private void FireSignal() {
            _signalBus.Fire(new PlayerHurtedSignal(gameObject));
        }
    }
}