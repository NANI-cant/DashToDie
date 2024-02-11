using CodeBase.Gameplay.Player;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CodeBase.UI.Elements {
    public class StaminaBar: MonoBehaviour {
        [SerializeField] private Slider _slider;

        private Stamina _targetStamina;

        [Inject]
        public void Construct(GameObject playerObject) {
            _targetStamina = playerObject.GetComponent<Stamina>();
        }

        private void OnEnable() {
            _targetStamina.Changed += UpdateUI;
            _targetStamina.Capacity.Modified += UpdateUI;
        }
        
        private void OnDisable() {
            _targetStamina.Changed -= UpdateUI;
            _targetStamina.Capacity.Modified -= UpdateUI;
        }

        private void Start() {
            UpdateUI();
        }

        private void UpdateUI() {
            _slider.minValue = 0;
            _slider.maxValue = _targetStamina.Capacity.Value;
            _slider.value = _targetStamina.Value;
        }
    }
}