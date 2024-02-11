using CodeBase.Gameplay.ProgressiveValues;
using CodeBase.ProjectContext.Services;
using UnityEngine;
using Zenject;

namespace CodeBase.Gameplay.Player {
    [RequireComponent(typeof(DirectionMover))]
    [RequireComponent(typeof(Stamina))]
    public class SlashCharger: MonoBehaviour {
        [SerializeField] private ProgressiveFloat _staminaPrice;
        
        private ITimeProvider _time;
        private IInputService _input;
        private DirectionMover _mover;
        private Stamina _stamina;
        private Transform _transform;
        private float _chargedDistance;

        public ProgressiveFloat StaminaPrice => _staminaPrice;
        public float ChargedDistance => _chargedDistance;

        [Inject]
        public void Construct(ITimeProvider timeProvider, IInputService input) {
            _time = timeProvider;
            _input = input;
        }

        private void Awake() {
            _mover = GetComponent<DirectionMover>();
            _stamina = GetComponent<Stamina>();
            _transform = transform;
        }

        public void Initialize(float staminaPrice) {
            _staminaPrice = new ProgressiveFloat(staminaPrice);
        }

        private void OnEnable() {
            _chargedDistance = 0;
            _time.SlowDown(10);
            _mover.Disable();
        }

        private void OnDisable() {
            _time.SpeedUp(10);
        }

        private void Update() {
            Ray pointerRay = _input.GetPointerRay(Camera.main);

            Plane characterPlane = _transform.GetPlane();
            if (!characterPlane.Raycast(pointerRay, out float rayLength)) {
                var inversePointerRay = new Ray(pointerRay.origin, -pointerRay.direction);
                characterPlane.Raycast(inversePointerRay, out rayLength);
            }

            Vector3 destination = pointerRay.GetPoint(rayLength);

            _transform.forward = _transform.position.DirectionTo(destination);
            _chargedDistance = Mathf.Min(
                Vector3.Distance(_transform.position, destination),
                _stamina.Value / _staminaPrice.Value
            );
        }
    }
}