using System;
using CodeBase.Gameplay.Player;
using CodeBase.ProjectContext.Services;
using UnityEngine;
using Zenject;

namespace CodeBase.Gameplay.General.Brains.Impl {
    public class PlayerInputBrain : MonoBehaviour, IBrain {
        [SerializeField] private DirectionMover _mover;
        [SerializeField] private Slasher _slasher;
        
        private IInputService _input;
        private Transform _transform;

        [Inject]
        public void Construct(IInputService input) {
            _input = input;
        }

        private void Awake() {
            _transform = transform;
        }

        public void Enable() => enabled = true;
        public void Disable() => enabled = false;

        private void OnEnable() {
            _input.SlashCalled += ExecuteSlash;
        }

        private void OnDisable() {
            _input.SlashCalled -= ExecuteSlash;
        }

        private void Update() {
            _mover.Move(_input.MoveDirection);

            HandleSlashing();
        }

        private void HandleSlashing() {
            if (!_input.Charging) return;
            
            Ray pointerRay = _input.GetPointerRay(Camera.main);
            Plane characterPlane = _transform.GetPlane();
            if (!characterPlane.Raycast(pointerRay, out float rayLength)) {
                var inversePointerRay = new Ray(pointerRay.origin, -pointerRay.direction);
                characterPlane.Raycast(inversePointerRay, out rayLength);
            }
            Vector3 destination = pointerRay.GetPoint(rayLength);
            
            _slasher.ChargeTo(destination);
        }

        private void ExecuteSlash() => _slasher.Execute();
    }
}