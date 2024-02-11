using UnityEngine;

namespace CodeBase.Gameplay.General {
    [RequireComponent(typeof(IHealth))]
    public class DeathProcessor: MonoBehaviour {
        private IHealth _health;
        private ICancelable[] _cancelableComponents;

        private void Awake() {
            _health = GetComponent<IHealth>();
            _cancelableComponents = GetComponents<ICancelable>();
        }

        private void OnEnable() => _health.Changed += TryDie;
        private void OnDisable() => _health.Changed -= TryDie;

        private void TryDie() {
            if (_health.HealthPoints > 0) return;

            foreach (var cancelable in _cancelableComponents) {
                cancelable.Cancel();
            }
            Destroy(gameObject);
        }
    }
}