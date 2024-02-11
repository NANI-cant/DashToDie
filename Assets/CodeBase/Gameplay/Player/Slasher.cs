using CodeBase.ProjectContext.Services;
using UnityEngine;
using Zenject;

namespace CodeBase.Gameplay.Player {
    [RequireComponent(typeof(SlashCharger))]
    [RequireComponent(typeof(SlashExecutor))]
    [RequireComponent(typeof(SlashRecovery))]
    public class Slasher : MonoBehaviour {
        private SlashRecovery _recovery;
        private SlashExecutor _executor;
        private SlashCharger _charger;
        private IInputService _input;

        [Inject]
        public void Construct(IInputService input) {
            _input = input;
        }

        private void Awake() {
            _charger = GetComponent<SlashCharger>();
            _executor = GetComponent<SlashExecutor>();
            _recovery = GetComponent<SlashRecovery>();
        }

        private void OnEnable() {
            _charger.Disable();
            _executor.Disable();
            _recovery.Enable();
        }
        
        private void OnDisable() {
            _charger.Disable();
            _executor.Disable();
            _recovery.Disable();
        }

        private void Update() {
            if(_recovery.enabled) return;
            
            if (_input.Charging && !_charger.enabled) {
                _charger.Enable();
                return;
            }

            if (!_input.Charging && _charger.enabled) {
                _charger.Disable();
                _executor.Enable();
                _recovery.Enable();
            }
        }
    }
}