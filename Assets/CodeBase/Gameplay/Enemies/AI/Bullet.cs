using System;
using CodeBase.Gameplay.General;
using CodeBase.ProjectContext.Services;
using UnityEngine;
using Zenject;

namespace CodeBase.Gameplay.Enemies.AI {
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class Bullet: MonoBehaviour {
        private Collider _trigger;
        private Rigidbody _rigidbody;
        private IDestroyProvider _destroyProvider;
        private int _damage;

        public int Damage => _damage;

        [Inject]
        public void Construct(IDestroyProvider destroyProvider) {
            _destroyProvider = destroyProvider;
        }

        private void Awake() {
            _trigger = GetComponent<Collider>();
            _rigidbody = GetComponent<Rigidbody>();

            _trigger.isTrigger = true;
        }

        public void Initialize(float speed, int damage) {
            _rigidbody.velocity = transform.forward * speed;
            _damage = damage;
        }

        private void OnTriggerEnter(Collider other) {
            if (other.TryGetComponent<IHurtable>(out var hurtable)) {
                hurtable.TakeHit(_damage, out _);
            }

            _destroyProvider.Destroy(gameObject);
        }
    }
}