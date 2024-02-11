using CodeBase.ProjectContext.Services;
using UnityEngine;
using Zenject;

namespace CodeBase.Gameplay.General.Impl {
    [RequireComponent(typeof(Rigidbody))]
    public class Rotator : MonoBehaviour, IRotator {
        [SerializeField][Min(0f)] private float _speed = 360f;
        
        private Transform _transform;
        private Rigidbody _rigidbody;
        private Vector3 _targetLookDirection;
        private IFixedTimeProvider _fixedTime;

        [Inject]
        public void Construct(IFixedTimeProvider fixedTimeProvider) {
            _fixedTime = fixedTimeProvider;
        }
        
        private void Awake() {
            _transform = transform;
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Start() => _targetLookDirection = transform.forward;

        private void FixedUpdate() {
            var fullAngle = Vector3.SignedAngle(_transform.forward, _targetLookDirection, Vector3.up);
            var direction = Mathf.Sign(fullAngle);
            var deltaAngle = direction * _speed * _fixedTime.FixedDelaTime;

            if (Mathf.Abs(deltaAngle) > Mathf.Abs(fullAngle)) deltaAngle = fullAngle;

            //_transform.Rotate(Vector3.up, deltaAngle, Space.World);
            _rigidbody.MoveRotation(_rigidbody.rotation * Quaternion.Euler(0, deltaAngle, 0));
        }

        public void RotateToPoint(Vector3 point) {
            var position = _transform.position;
            point.y = position.y;
            RotateToDirection(position.DirectionTo(point));
        }

        public void RotateToDirection(Vector3 direction) => _targetLookDirection = direction;
    }
}