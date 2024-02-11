using System;
using CodeBase.ProjectContext.Services;
using UnityEngine;
using Zenject;

namespace CodeBase.Gameplay.Player {
    [RequireComponent(typeof(Rigidbody))]
    public class DirectionMover: MonoBehaviour {
        [SerializeField] [Min(0)] private float _speed = 2;

        [Header("View")]
        [SerializeField] private Animator _animator;
        
        private Transform _transform;
        private IFixedTimeProvider _fixedTime;
        private Rigidbody _rigidbody;
        private Vector3 _moveDirection;

        [Inject]
        public void Construct(IFixedTimeProvider fixedTimeProvider) {
            _fixedTime = fixedTimeProvider;
        }

        private void Awake() {
            _transform = transform;
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void OnEnable() => _animator.SetFloat("MoveSpeed", 1);
        private void OnDisable() => _animator.SetFloat("MoveSpeed", 0);

        public void Move(Vector3 moveDirection) {
            _moveDirection = moveDirection;
        }

        private void FixedUpdate() {
            if (_moveDirection == Vector3.zero) {
                _animator.SetFloat("MoveSpeed", 0);
                return;
            }

            _animator.SetFloat("MoveSpeed", 1);
            Vector3 globalDirection = Camera.main.transform.TransformDirection(_moveDirection);
            Vector3 xZDirection = globalDirection.ToXZPlane().normalized;
            
            _transform.forward = xZDirection;
            _rigidbody.MovePosition(_rigidbody.position + xZDirection * _speed * _fixedTime.FixedDelaTime);
        }
    }
}