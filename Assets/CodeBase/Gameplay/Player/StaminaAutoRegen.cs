using CodeBase.Gameplay.ProgressiveValues;
using CodeBase.ProjectContext.Services;
using UnityEngine;
using Zenject;

namespace CodeBase.Gameplay.Player {
    [RequireComponent(typeof(Stamina))]
    public class StaminaAutoRegen: MonoBehaviour {
        [SerializeField] private ProgressiveFloat _staminaPerSecond;
        
        private ITimeProvider _time;
        private Stamina _stamina;

        public ProgressiveFloat StaminaPerSecond => _staminaPerSecond;

        [Inject]
        public void Construct(ITimeProvider timeProvider) {
            _time = timeProvider;
        }

        private void Awake() {
            _stamina = GetComponent<Stamina>();
        }

        private void Update() {
            _stamina.Gain(_staminaPerSecond.Value * _time.DelaTime);
        }
    }
}