using CodeBase.Gameplay.General.Brains;
using CodeBase.Gameplay.General.Impl;
using UnityEngine;
using UnityEngine.AI;

namespace CodeBase.Gameplay.Enemies.AI {
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Rotator))]
    public class EnemyBrain : MonoBehaviour, IBrain {
        [SerializeField] protected Animator _animator;
        [SerializeField] private LayerMask _obstacles;
        [SerializeField] [Min(0)] private float _approaching = 0.75f;
        
        protected Transform Transform { get; private set; }
        protected NavMeshAgent Agent { get; private set; }
        protected Rotator Rotator { get; private set; }
        protected Animator Animator => _animator;

        protected virtual void Awake() {
            Agent = GetComponent<NavMeshAgent>();
            Rotator = GetComponent<Rotator>();
            Transform = transform;
        }

        public void Enable() {
            Agent.Enable();
            enabled = true;
        }

        public void Disable() {
            Agent.Disable();
            enabled = false;
        }

        protected bool CanSeeTarget(Transform target) {
            Ray checkingRay = new Ray(transform.position, transform.position.DirectionTo(target.position));
            float distance = Vector3.Distance(transform.position, target.position);
            return !Physics.Raycast(checkingRay, distance, _obstacles);
        }

        protected bool CheckApproachedTarget(Transform target) {
            return Vector3.Distance(target.position, Transform.position) <= _approaching;
        }

#if UNITY_EDITOR
        private void OnValidate() {
            if (_animator == null) _animator = GetComponentInChildren<Animator>();
        }
#endif
    }
}