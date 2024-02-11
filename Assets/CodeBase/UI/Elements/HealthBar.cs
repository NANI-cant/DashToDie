using CodeBase.Gameplay.General;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CodeBase.UI.Elements {
    public class HealthBar: MonoBehaviour {
        [SerializeField] private Slider _slider;
        
        private IHealth _trackedHealth;

        [Inject]
        public void Construct(GameObject playerObject) {
            _trackedHealth = playerObject.GetComponent<IHealth>();
        }

        private void OnEnable() => _trackedHealth.Changed += UpdateUI;
        private void OnDisable() => _trackedHealth.Changed -= UpdateUI;

        private void Start() => UpdateUI();

        private void UpdateUI() {
            _slider.maxValue = _trackedHealth.MaxHealth;
            _slider.value = _trackedHealth.HealthPoints;
        }
    }
}