using CodeBase.Gameplay.Player;
using CodeBase.ProjectContext.Services;
using UnityEngine;
using Zenject;

namespace CodeBase.Gameplay.General.Brains.Impl {
    public class PlayerInputBrain : MonoBehaviour, IBrain {
        [SerializeField] private DirectionMover _mover;
        [SerializeField] private NewSlasher _slasher;
        
        private IInputService _input;

        [Inject]
        public void Construct(IInputService input) {
            _input = input;
        }
        
        public void Enable() {
            enabled = true;
            _input.SlashCharged += ChargeSlash;
            _input.SlashExecuted += ExecuteSlash;
        }

        public void Disable() {
            enabled = false;
        }

        private void Update() {
            _mover.Move(_input.MoveDirection);
        }

        private void ChargeSlash() {
            _slasher.Charge();
        }

        private void ExecuteSlash() {
            _slasher.Execute();
        }
    }
}