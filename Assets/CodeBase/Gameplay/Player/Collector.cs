using System;
using CodeBase.Gameplay.Environment.Collectables;
using UnityEngine;

namespace CodeBase.Gameplay.Player {
    public class Collector: MonoBehaviour {
        [SerializeField] private LayerMask _collectable;
        
        private Transform _transform;
        private Vector3 _lastPosition;

        private void Awake() {
            _transform = transform;
        }

        private void Start() {
            _lastPosition = _transform.position;
        }

        private void Update() {
            var nowPosition = _transform.position;
            if (Vector3.Distance(_lastPosition, nowPosition) == 0) {
                _lastPosition = nowPosition;
                return;
            }
            
            Vector3 center = (_lastPosition + nowPosition) / 2 + Vector3.up * 0.5f;
            Vector3 halfExtends = new Vector3(Mathf.Abs(nowPosition.x - _lastPosition.x) / 2, 0.5f, Mathf.Abs(nowPosition.z - _lastPosition.z) / 2);
            Collider[] collectableColliders = Physics.OverlapBox(center, halfExtends, Quaternion.identity, _collectable, QueryTriggerInteraction.Collide);

            foreach (var collectCollider in collectableColliders) {
                if(!collectCollider.TryGetComponent<ICollectable>(out var collectable)) continue;
                collectable.Collect(gameObject);
            }
            
            _lastPosition = nowPosition;
        }
    }
}