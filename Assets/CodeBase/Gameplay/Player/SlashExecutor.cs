using System;
using System.Collections.Generic;
using CodeBase.Gameplay.General;
using CodeBase.Gameplay.VFX;
using CodeBase.ProjectContext.Services;
using UnityEngine;
using Zenject;

namespace CodeBase.Gameplay.Player {
    [RequireComponent(typeof(IDamageDealer))]
    [RequireComponent(typeof(SlashCharger))]
    [RequireComponent(typeof(Stamina))]
    [RequireComponent(typeof(Rigidbody))]
    public class SlashExecutor: MonoBehaviour {
        [SerializeField] private Animator _animator;
        [SerializeField] private MeshTrail _trail;
        
        [SerializeField] private Collider _hurtBox;
        [SerializeField][Min(0)] private float _speed = 100;
        [SerializeField] private Transform _slashOrigin;
        [SerializeField] private LayerMask _obstacles;
        [SerializeField] private LayerMask _enemies;
        
        private readonly List<IHurtable> _hurtOnSlashCache = new();
        private IDamageDealer _damageDealer;
        private Transform _transform;
        private SlashCharger _charger;
        private Stamina _stamina;
        private Rigidbody _rigidbody;
        private IFixedTimeProvider _fixedTime;

        public Transform SlashOrigin => _slashOrigin;

        public float SlashDistance { get; private set; }

        [Inject]
        public void Construct(IFixedTimeProvider fixedTime) {
            _fixedTime = fixedTime;
        }

        private void Awake() {
            _damageDealer = GetComponent<IDamageDealer>();
            _charger = GetComponent<SlashCharger>();
            _transform = transform;
            _rigidbody = GetComponent<Rigidbody>();
            _stamina = GetComponent<Stamina>();
            
            _trail.Disable();
        }

        private void OnEnable() {
            _animator.SetTrigger("Slash");
            _trail.Enable();
            _hurtBox.enabled = false;

            SlashDistance = _charger.ChargedDistance;
            if (Physics.Raycast(SlashOrigin.position, SlashOrigin.forward, out var raycastHit, SlashDistance, _obstacles)) {
                SlashDistance = raycastHit.distance;
            }

            _stamina.Spend(SlashDistance * _charger.StaminaPrice.Value);
            _hurtOnSlashCache.Clear();
        }

        private void OnDisable() {
            _trail.Disable();
            _hurtBox.enabled = true;
            SlashDistance = 0;
        }

        private void FixedUpdate() {
            if (SlashDistance <= 0) {
                this.Disable();
            }
            
            float translationDistance = _speed * _fixedTime.FixedDelaTime;
            translationDistance = Mathf.Min(translationDistance, SlashDistance);
            
            Vector3 lastPosition = _rigidbody.position;
            _rigidbody.MovePosition(lastPosition + _transform.forward * translationDistance);
            Vector3 newPosition = _rigidbody.position;

            HitOnPath(lastPosition, newPosition);

            SlashDistance -= translationDistance;
        }
        
        private void HitOnPath(Vector3 from, Vector3 to) {
            Vector3 center = (from + to) / 2 + Vector3.up * 0.5f;
            Vector3 halfExtends = new Vector3(Mathf.Abs(to.x - from.x) / 2, 0.5f, Mathf.Abs(to.z - from.z) / 2);
            Collider[] collidersOnPath = Physics.OverlapBox(center, halfExtends, Quaternion.identity, _enemies, QueryTriggerInteraction.Collide);

            foreach (var hurtableCollider in collidersOnPath) {
                if(!hurtableCollider.TryGetComponent<IHurtable>(out var hurtable)) continue;
                if(_hurtOnSlashCache.Contains(hurtable)) continue;
                
                hurtable.TakeHit(_damageDealer.Damage.Value, out _);
                _hurtOnSlashCache.Add(hurtable);
            }
        }
    }
}